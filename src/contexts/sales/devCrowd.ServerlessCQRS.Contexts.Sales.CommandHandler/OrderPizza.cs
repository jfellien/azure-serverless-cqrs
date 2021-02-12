using System;
using System.IO;
using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Contexts.Sales.CommandHandler.Models;
using devCrowd.ServerlessCQRS.Core.Events.Sales;
using devCrowd.ServerlessCQRS.CustomBindings.EventStore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.CommandHandler
{
    public static class OrderPizza
    {
        [FunctionName("OrderPizza")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "order")]
            Order order,
            [DomainEventStream("Sales", "Order")]
            DomainEventStream eventStream,
            ILogger log)
        {
            var orderPlaced = new OrderPlaced
            {
                OrderId = Guid.NewGuid().ToString(),
                OrderNumber = NextOrderNumber(),
                Paste = order.Paste,
                Tomatoes = order.Tomatoes,
                Cheese = order.Cheese,
                Amount = order.Amount
            };

            // Put the Event into EventStream.
            // EntityId is needed because the Stream doesn't know the Id and can't read from Event 
            await eventStream.Append(orderPlaced, orderPlaced.OrderId);

            return new OkObjectResult(new
            {
                orderNumber = orderPlaced.OrderNumber,
                orderId = orderPlaced.OrderId,
                message = "Order placed"
            });
        }

        /// <summary>
        /// Sample Method to generate a new Order Number
        /// </summary>
        /// <returns></returns>
        private static string NextOrderNumber()
        {
            return $"PF-{DateTime.Now.ToString("d")}-{DateTime.Now.ToString("hh/mm/ss")}";
        }
    }
}