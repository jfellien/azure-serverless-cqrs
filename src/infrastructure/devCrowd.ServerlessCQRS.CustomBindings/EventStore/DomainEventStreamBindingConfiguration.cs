using System;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    [Extension("DomainEventStream")]
    public class DomainEventStreamBindingConfiguration : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            context
                .AddBindingRule<DomainEventStreamAttribute>()
                .BindToInput<DomainEventStream>(GetFromAttribute);
        }

        private DomainEventStream GetFromAttribute(DomainEventStreamAttribute attribute)
        {
            var eventStoreConnectionString = Environment.GetEnvironmentVariable("EVENT_STORE_CONNECTION_STRING");
            var eventStoreDatabaseName = Environment.GetEnvironmentVariable("EVENT_STORE_DB_NAME");
            var eventsCollectionName = Environment.GetEnvironmentVariable("DOMAIN_EVENTS_COLLECTION_NAME");
            
            if (string.IsNullOrEmpty(eventStoreConnectionString))
            {
                throw new ArgumentException("EVENT_STORE_CONNECTION_STRING not set in Application Settings");
            }
            
            var domainEventStreamStorage = new CosmosDbDomainEventStreamStorage(
                eventStoreConnectionString, 
                eventStoreDatabaseName, 
                eventsCollectionName);
            
            var domainEventsPublisher = new ServiceBusDomainEventsPublisher();
            
            return new DomainEventStream(
                attribute.ContextName,
                attribute.EntityName,
                attribute.EntityId,
                domainEventStreamStorage,
                domainEventsPublisher);
        }
    }
}