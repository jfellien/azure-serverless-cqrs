﻿using System;
using System.Text;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace devCrowd.ServerlessCQRS.Infrastructure.Lib
{
    public static class ServiceBusMessageConverter
    {
        const string EVENT_TYPE = "ContainedEventType";

        /// <summary>
        /// Converts a DomainEvent to a ServiceBusMessage and sets
        /// a UserProperty with EventType Name
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        public static Message ToServiceBusMessage(IDomainEvent domainEvent)
        {
            var eventAsString = JsonConvert.SerializeObject(domainEvent);
            var eventAsBytes = Encoding.UTF8.GetBytes(eventAsString);
            
            var serviceBusMessage = new Message(eventAsBytes);
            var eventTypeName = domainEvent.GetType().AssemblyQualifiedName;
            
            serviceBusMessage.UserProperties.Add(EVENT_TYPE, eventTypeName);

            return serviceBusMessage;
        }
        
        /// <summary>
        /// Converts a ServiceBusMessage to a DomainEvent
        /// </summary>
        /// <param name="serviceBusMessage"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If UserProperty does not contain a Event Type property</exception>
        /// <exception cref="ArgumentException">If Event Type not available in solution</exception>
        public static IDomainEvent ToDomainEvent(Message serviceBusMessage)
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
            catch (Exception e)
            {
                throw;
            }
        }
    }
}