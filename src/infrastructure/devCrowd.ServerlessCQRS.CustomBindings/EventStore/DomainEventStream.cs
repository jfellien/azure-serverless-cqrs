using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    public class DomainEventStream
    {
        private readonly string _context;
        private readonly string _entity;
        private readonly IReadAndWriteDomainEvents _storage;
        private readonly IPublishDomainEvents _publisher;

        private string _entityId;
        private DomainEventSequence _historySequence;

        public DomainEventStream(
            string context, string entity, string entityId, 
            IReadAndWriteDomainEvents storage, 
            IPublishDomainEvents publisher)
        {
            _context = context;
            _entity = entity;
            _entityId = entityId;
            _storage = storage;
            _publisher = publisher;

            _historySequence = new DomainEventSequence();
        }

        public Task Append(object domainEvent)
        {
            return Append(new List<object>
            {
                domainEvent
            });
        }
        
        public Task Append(object domainEvent, string entityId)
        {
            return Append(new List<object>
            {
                domainEvent
            }, entityId);
        }

        public async Task Append(IEnumerable<object> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                _historySequence.Append(domainEvent);
                
                // Write Event to Storage. If _entityId is null, so its intentional.
                await _storage.Write(domainEvent, _context, _entity, _entity);
            }
        }
        
        public async Task Append(IEnumerable<object> domainEvents, string entityId)
        {
            if (string.IsNullOrWhiteSpace(entityId))
            {
                throw new ArgumentNullException(entityId);
            }
            
            if (string.IsNullOrWhiteSpace(_entityId) == false 
                && string.IsNullOrWhiteSpace(entityId) == false
                && _entity.ToLowerInvariant() != entityId.ToLowerInvariant())
            {
                throw new ArgumentException("You have set entityId but this instance of EventStream already have an EntityId");
            }
            
            _entityId = entityId;
            
            foreach (var domainEvent in domainEvents)
            {
                // Write Event to Storage. EntityId has a value.
                var sequenceNumber = await _storage.Write(domainEvent, _context, _entity, _entityId);
                
                _historySequence.Add(new SequencedDomainEvent(sequenceNumber, domainEvent));
            }
        }

        public async Task<bool> IsEmpty()
        {
            if (_historySequence == null)
            {
                _historySequence = await GetFromStorageByGivenParameters();
            }
            
            return _historySequence.Count() == 0;
        }

        private Task<DomainEventSequence> GetFromStorageByGivenParameters()
        {
            if (string.IsNullOrWhiteSpace(_entity)
                && string.IsNullOrWhiteSpace(_entityId))
            {
                return _storage.ReadBy(_context, default);
            }

            if (string.IsNullOrWhiteSpace(_entity) == false
                && string.IsNullOrWhiteSpace(_entityId))
            {
                return _storage.ReadBy(_context, _entity, default);
            }

            return _storage.ReadBy(_context, _entity, _entityId, default);
        }
    }
}