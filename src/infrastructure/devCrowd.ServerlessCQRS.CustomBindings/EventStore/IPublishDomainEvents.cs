using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    public interface IPublishDomainEvents
    {
        Task Publish(object domainEvent);
    }
}