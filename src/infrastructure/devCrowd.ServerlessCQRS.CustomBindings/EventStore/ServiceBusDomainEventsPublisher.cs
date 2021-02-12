using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Infrastructure.Lib;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.Extensions;
using Microsoft.Azure.ServiceBus;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    internal class ServiceBusDomainEventsPublisher : IPublishDomainEvents
    {
        private readonly TopicClient _topicClient;

        public ServiceBusDomainEventsPublisher(string connectionString, string topic)
        {
            _topicClient = new TopicClient(connectionString, topic);
        }
        
        public Task Publish(IDomainEvent domainEvent)
        {
            var eventAsMessage = domainEvent.ToServiceBusMessage();

            return _topicClient.SendAsync(eventAsMessage);
        }
    }
}