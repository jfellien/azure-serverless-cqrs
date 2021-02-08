using System.Collections.Generic;
using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    public interface IReadAndWriteDomainEvents
    {
        Task<IEnumerable<object>> ReadFrom(string context);
        Task<IEnumerable<object>> ReadFrom(string context, string entity);
        Task<IEnumerable<object>> ReadFrom(string context, string entity, string id);
    }
}