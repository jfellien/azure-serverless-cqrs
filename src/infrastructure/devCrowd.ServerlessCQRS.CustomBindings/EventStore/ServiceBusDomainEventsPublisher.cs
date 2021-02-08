using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    internal class ServiceBusDomainEventsPublisher : IPublishDomainEvents
    {
        public Task Publish(object domainEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}