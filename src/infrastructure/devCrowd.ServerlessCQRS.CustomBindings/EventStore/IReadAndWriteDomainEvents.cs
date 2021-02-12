using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    public interface IReadAndWriteDomainEvents
    {
        Task<DomainEventSequence> ReadBy(string context, CancellationToken cancellationToken);
        Task<DomainEventSequence> ReadBy(string context, string entity, CancellationToken cancellationToken);
        Task<DomainEventSequence> ReadBy(string context, string entity, string entityId, CancellationToken cancellationToken);
        /// <summary>
        /// Writes the Event into Storage and returns it SequenceNumber.
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <param name="entityId"></param>
        /// <returns>This Events SequenceNumber</returns>
        Task<long> Write(object domainEvent, string context, string entity = null, string entityId = null);
    }
}