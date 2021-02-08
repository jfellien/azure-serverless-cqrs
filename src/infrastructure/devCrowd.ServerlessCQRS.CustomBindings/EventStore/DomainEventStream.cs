using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    public class DomainEventStream
    {
        private readonly IReadAndWriteDomainEvents _storage;
        private readonly IPublishDomainEvents _publisher;

        public DomainEventStream(string context, string entity, string id, 
            IReadAndWriteDomainEvents storage, IPublishDomainEvents publisher)
        {
            _storage = storage;
            _publisher = publisher;
        }

        public Task<bool> Exists()
        {
            return Task.FromResult<bool>(false);
        }
        
        public void Append(object domainEvent)
        {
            
        }

        public void Append(IEnumerable<object> domainEvents)
        {
            
        }
    }
}