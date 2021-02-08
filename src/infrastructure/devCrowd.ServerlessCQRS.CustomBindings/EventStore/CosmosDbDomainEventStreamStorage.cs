using System.Collections.Generic;
using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    internal class CosmosDbDomainEventStreamStorage : IReadAndWriteDomainEvents
    {
        public Task<IEnumerable<object>> ReadFrom(string context)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<object>> ReadFrom(string context, string entity)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<object>> ReadFrom(string context, string entity, string id)
        {
            throw new System.NotImplementedException();
        }
    }
}