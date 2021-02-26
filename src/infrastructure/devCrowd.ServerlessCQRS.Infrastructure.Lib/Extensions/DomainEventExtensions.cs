using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

#nullable enable

namespace devCrowd.ServerlessCQRS.Infrastructure.Lib.Extensions
{
    public static class DomainEventExtensions
    {
        /// <summary>
        /// Converts a DomainEvent to a ServiceBusMessage and sets
        /// a UserProperty with EventType Name
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        public static Message ToServiceBusMessage(this IDomainEvent domainEvent)
        {
            var eventAsString = JsonConvert.SerializeObject(domainEvent);
            var eventAsBytes = Encoding.UTF8.GetBytes(eventAsString);

            var serviceBusMessage = new Message(eventAsBytes);
            var eventTypeName = domainEvent.GetType().AssemblyQualifiedName;

            serviceBusMessage.UserProperties.Add(ServiceBusMessageExtensions.EVENT_TYPE, eventTypeName);

            return serviceBusMessage;
        }

        public static Task HandleMeWith(this IDomainEvent domainEvent, IHandleEvents eventHandler)
        {
            var handlerType = eventHandler.GetType();
            var eventType = domainEvent.GetType();

            var handlerMethodsForEventType =
                from method in handlerType.GetTypeInfo().DeclaredMethods
                from parameter in method.GetParameters()
                where method.IsPublic
                where parameter.ParameterType == eventType
                select method;

            var allHandlerMethods = handlerMethodsForEventType.ToList();

            if (handlerMethodsForEventType.Any() == false)
            {
                throw new ArgumentException($"This Event Type {eventType.Name} has no Handler Method");
            }

            if (handlerMethodsForEventType.Count() > 1)
            {
                throw new ArgumentException($"This Event Type {eventType.Name} has to much Handler Methods");
            }

            // Invoke the one and only Handle([Event Type]) Method
            return (Task)handlerMethodsForEventType
                .Single()
                .Invoke(eventHandler, new object?[] {domainEvent});
        }
    }
}