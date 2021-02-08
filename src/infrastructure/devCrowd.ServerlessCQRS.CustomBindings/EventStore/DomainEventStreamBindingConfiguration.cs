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
            var domainEventStreamStorage = new CosmosDbDomainEventStreamStorage();
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