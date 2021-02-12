using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    internal class CosmosDbDomainEventStreamStorage : IReadAndWriteDomainEvents
    {
        private bool _domainEventStreamHasBeenRead;
        private long _lastSequenceNumberOfStream;
        
        private Container _domainEventsContainer;
        private JsonSerializer _defaultSerializer;

        public CosmosDbDomainEventStreamStorage(string connectionString, string dbName, string collectionName)
        {
            _domainEventsContainer = new CosmosClient(connectionString).GetContainer(dbName, collectionName);

            var serializerSettings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None
            };

            _defaultSerializer = JsonSerializer.CreateDefault(serializerSettings);
        }

        public Task<DomainEventSequence> ReadBy(string context, CancellationToken cancellationToken)
        {
            var query = new QueryDefinition(
                    "SELECT * FROM c " +
                    "WHERE c.context=@context " +
                    "ORDER BY c.isoTimestamp")
                .WithParameter("@context", context.ToLowerInvariant());

            return ReadDomainEventsStream(query, cancellationToken);
        }

        public Task<DomainEventSequence> ReadBy(string context, string entity, CancellationToken cancellationToken)
        {
            var query = new QueryDefinition(
                    "SELECT * FROM c " +
                    "WHERE c.context=@context " +
                    "AND c.entity=@entity " +
                    "ORDER BY c.isoTimestamp")
                .WithParameter("@context", context.ToLowerInvariant())
                .WithParameter("@entity", entity.ToLowerInvariant());

            return ReadDomainEventsStream(query, cancellationToken);
        }

        public Task<DomainEventSequence> ReadBy(string context, string entity, string entityId, CancellationToken cancellationToken)
        {
            var query = new QueryDefinition(
                    "SELECT * FROM c " +
                    "WHERE c.context=@context " +
                    "AND c.entity=@entity " +
                    "AND c.entityId=@entityId " +
                    "ORDER BY c.isoTimestamp")
                .WithParameter("@context", context.ToLowerInvariant())
                .WithParameter("@entity", entity.ToLowerInvariant())
                .WithParameter("@entityId", entityId);

            return ReadDomainEventsStream(query, cancellationToken);
        }

        public async Task<long> Write(IDomainEvent domainEvent, string context, string entity = null, string entityId = null)
        {
            var domainEventWrap = new EventStoreDomainEventWrap
            {
                EventId = Guid.NewGuid().ToString(),

                Context = context.ToLowerInvariant(),
                Entity = entity.ToLowerInvariant(),
                EntityId = entityId,

                EventName = domainEvent.GetType().Name,
                EventFullName = domainEvent.GetType().AssemblyQualifiedName,
                IsoTimestamp = DateTime.UtcNow.ToString("O"),
                SequenceNumber = await NextSequenceNumber(context, entity, entityId),

                DomainEvent = domainEvent
            };

            await _domainEventsContainer.CreateItemAsync(domainEventWrap);

            return domainEventWrap.SequenceNumber;
        }

        private async Task<DomainEventSequence> ReadDomainEventsStream(QueryDefinition query, CancellationToken cancellationToken)
        {
            var events = new DomainEventSequence();

            using (var resultSet = _domainEventsContainer.GetItemQueryIterator<JObject>(query))
            {
                while (resultSet.HasMoreResults)
                {
                    var wrappedDomainEvents = await resultSet.ReadNextAsync(cancellationToken);

                    var sequencedDomainEvents = wrappedDomainEvents.Select(ConvertFromEventData);

                    events.AddRange(sequencedDomainEvents);

                    _lastSequenceNumberOfStream = events.Any() 
                        ? events.Last().SequenceNumber
                        : 0;
                }
            }

            // We need this because of LastSequenceNumber.
            // If we don't have read the stream we have no number for the Write operation
            // where we need the next sequence number.
            _domainEventStreamHasBeenRead = true;

            return events;
        }

        private SequencedDomainEvent ConvertFromEventData(JObject wrappedDomainEvent)
        {
            var eventType = wrappedDomainEvent["eventFullName"].ToString();

            var domainEventType = Type.GetType(eventType);

            if (domainEventType == null)
            {
                throw new TypeAccessException(
                    $"Deserialization Error: Event Type {eventType} is not part of Events anymore");
            }

            var sequenceNumber = wrappedDomainEvent["sequenceNumber"].Value<long>();
            var domainEventInstance = wrappedDomainEvent["payload"].ToObject(domainEventType, _defaultSerializer) as IDomainEvent;

            return new SequencedDomainEvent(sequenceNumber, domainEventInstance);
        }

        /// <summary>
        /// Get the next Sequence Number of Stream. A Stream have different identifier.
        /// Its always a combination of Context, Entity and EntityID. Entity and EntityID can be null or empty.
        /// So a Stream can be a Context-Stream, a Context-Entity-Stream or a Context-Entity-EntityID-Stream. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        private async Task<long> NextSequenceNumber(string context, string entity, string entityId)
        {
            // If we don't have read the stream already,
            // first we need the last sequence number from database
            // by get the MAX value
            if (_domainEventStreamHasBeenRead == false)
            {
                _lastSequenceNumberOfStream = await GetLastSequenceNumberOfStream(context, entity, entityId);
            }

            // I don't know why, but a simple _lastSequenceNumberOfStream++ doesn't work
            _lastSequenceNumberOfStream = _lastSequenceNumberOfStream + 1;

            return _lastSequenceNumberOfStream;
        }

        private async Task<long> GetLastSequenceNumberOfStream(string context, string entity, string entityId)
        {
            long lastSequenceNumber = 0;

            var queryDefinition = new QueryDefinition(BuildQueryStringForLastSequenceNumber(context, entity, entityId))
                .WithParameter("@context", context.ToLower());

            if (string.IsNullOrWhiteSpace(entity) == false)
            {
                queryDefinition.WithParameter("@entity", entity.ToLower());
            }

            if (string.IsNullOrWhiteSpace(entityId) == false)
            {
                queryDefinition.WithParameter("@entityId", entityId.ToLower());
            }

            using (var resultSet =
                _domainEventsContainer.GetItemQueryIterator<LastSequenceNumberQueryResult>(queryDefinition))
            {
                while (resultSet.HasMoreResults)
                {
                    var scalarResult = await resultSet.ReadNextAsync();

                    lastSequenceNumber = scalarResult.Count == 1 ? scalarResult.First().MaxSequenceNumber : 0;
                }
            }

            return lastSequenceNumber;
        }

        private string BuildQueryStringForLastSequenceNumber(string context, string entity, string entityId)
        {
            var queryString = $"SELECT MAX(c.sequenceNumber) AS maxSequenceNumber FROM c "
                              + "WHERE c.context = @context";

            if (string.IsNullOrWhiteSpace(entity) == false)
            {
                queryString += " AND c.entity = @entity";
            }

            if (string.IsNullOrWhiteSpace(entityId) == false)
            {
                queryString += " AND c.entityId = @entityId";
            }

            queryString += " ORDER BY c.isoTimeStamp";

            return queryString;
        }
    }
}