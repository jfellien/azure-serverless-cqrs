using System;
using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Services;
using devCrowd.ServerlessCQRS.Infrastructure.Lib;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler
{
    public class ReceiveMessage
    {
        private readonly IHandleSalesEvents _salesEventHandler;

        public ReceiveMessage(IHandleSalesEvents salesEventHandler)
        {
            _salesEventHandler = salesEventHandler;
        }
        
        [FunctionName("HandleEvent")]
        [ServiceBusAccount("EVENT_HANDLER_CONNECTION_STRING")]
        public async Task RunAsync(
            [ServiceBusTrigger(
                "%EVENT_HANDLER_TOPIC_NAME%", 
                "%EVENT_HANDLER_SUBSCRIPTION_NAME%")]
            Message serviceBusMMessage, 
            ILogger log)
        {
            try
            {
                var domainEvent = ServiceBusMessageConverter.ToDomainEvent(serviceBusMMessage);
                //_salesEventHandler.Handle(domainEvent);
            }
            catch (Exception e)
            {
                log.LogError(e,e.Message);
                throw;
            }
        }
    }
}