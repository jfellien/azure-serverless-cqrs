using System;
using System.Text;
using devCrowd.CustomBindings.EventSourcing.EventStreamStorages;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace devCrowd.ServerlessCQRS.Core.EventHandling
{
    public static class ServiceBusMessageExtensions
    {
        public const string EVENT_TYPE = "ContainedEventType";

        /// <summary>
        /// Converts a ServiceBusMessage to a DomainEvent
        /// </summary>
        /// <param name="serviceBusMessage"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If UserProperty does not contain a Event Type property</exception>
        /// <exception cref="ArgumentException">If Event Type not available in solution</exception>
        public static IDomainEvent ToDomainEvent(this Message serviceBusMessage)
        {
            if (serviceBusMessage.UserProperties.ContainsKey(EVENT_TYPE) == false)
            {
                throw new ArgumentException($"Message does not contain UserProperty '{EVENT_TYPE}'");
            }

            var eventTypeName = serviceBusMessage.UserProperties[EVENT_TYPE].ToString();

            var eventType = Type.GetType(eventTypeName, false, true);

            if (eventType == null)
            {
                throw new ArgumentException($"Can't find Event Type: {eventTypeName} in current solution.");
            }

            var messageAsString = Encoding.UTF8.GetString(serviceBusMessage.Body);

            try
            {
                return JsonConvert.DeserializeObject(messageAsString, eventType) as IDomainEvent;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}