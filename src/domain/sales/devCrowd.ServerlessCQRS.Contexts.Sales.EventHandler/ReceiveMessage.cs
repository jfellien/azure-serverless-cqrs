using System;
using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Core.EventHandling;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler
{
    public class ReceiveMessage
    {
        private readonly IHandleEvents _salesEventHandler;

        public ReceiveMessage(IHandleEvents salesEventHandler)
        {
            _salesEventHandler = salesEventHandler;
        }
        
        [FunctionName("HandleEvent")]
        [ServiceBusAccount("EVENT_HANDLER_CONNECTION_STRING")]
        public async Task RunAsync(
            [ServiceBusTrigger(
                "%EVENT_HANDLER_TOPIC_NAME%", 
                "%EVENT_HANDLER_SUBSCRIPTION_NAME%")]
            Message serviceBusMessage, 
            ILogger log)
        {
            try
            {
                var domainEvent = serviceBusMessage.ToDomainEvent();
                
                await _salesEventHandler.Handle(domainEvent);
            }
            catch (Exception e)
            {
                log.LogError(e,e.Message);
                throw;
            }
        }
    }
}