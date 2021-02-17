using System;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;

namespace devCrowd.ServerlessCQRS.CustomBindings.Notifications
{
    [Extension("Broadcaster")]
    public class BroadcasterBindingConfiguration : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            context
                .AddBindingRule<BroadcasterAttribute>()
                .BindToInput<Broadcaster>(GetFromConfig);
        }

        private Broadcaster GetFromConfig(BroadcasterAttribute attribute)
        {
            var connectionString = Environment.GetEnvironmentVariable("BROADCASTER_CONNECTION_STRING");
            var infoQueueName = Environment.GetEnvironmentVariable("BROADCASTER_INFO_QUEUE");
            var warningQueueName = Environment.GetEnvironmentVariable("BROADCASTER_WARNING_QUEUE");
            var errorQueueName = Environment.GetEnvironmentVariable("BROADCASTER_ERROR_QUEUE");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ApplicationException("Missing Connection String for Broadcaster in Application Settings. " +
                                               "You need an Azure Service Bus Connection String Setting 'BROADCASTER_CONNECTION_STRING'");
            }

            if (string.IsNullOrWhiteSpace(infoQueueName))
            {
                throw new ApplicationException(
                    "Missing Info Queue Name Setting for Broadcaster in Application Settings. " +
                    "You need an Azure Service Bus Queue Name Setting 'BROADCASTER_INFO_QUEUE'");
            }
            
            if (string.IsNullOrWhiteSpace(warningQueueName))
            {
                throw new ApplicationException(
                    "Missing Warning Queue Name Setting for Broadcaster in Application Settings. " +
                    "You need an Azure Service Bus Queue Name Setting 'BROADCASTER_WARNING_QUEUE'");
            }
            
            if (string.IsNullOrWhiteSpace(errorQueueName))
            {
                throw new ApplicationException(
                    "Missing Error Queue Name Setting for Broadcaster in Application Settings. " +
                    "You need an Azure Service Bus Queue Name Setting 'BROADCASTER_ERROR_QUEUE'");
            }

            return new Broadcaster(connectionString, infoQueueName, warningQueueName, errorQueueName);
        }
    }
}