using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using devCrowd.CustomBindings.EventSourcing.EventStreamStorages;

namespace devCrowd.ServerlessCQRS.Core.EventHandling
{
    public static class DomainEventExtensions
    {
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