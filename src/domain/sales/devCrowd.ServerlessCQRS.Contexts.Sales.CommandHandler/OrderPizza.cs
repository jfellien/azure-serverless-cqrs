using System;
using System.Threading.Tasks;
using devCrowd.CustomBindings.EventSourcing;
using devCrowd.ServerlessCQRS.Contexts.Sales.CommandHandler.Models;
using devCrowd.ServerlessCQRS.Core.Events.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.CommandHandler
{
    public static class OrderPizza
    {
        [FunctionName("Order")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "order")]
            Order order,
            [DomainEventStream("Sales", "Order")]
            DomainEventStream eventStream,
            ILogger log)
        {
            /// Here is the place for business validation.
            /// If an Order is correct, we create the event OrderAccepted

            var orderId = Guid.NewGuid().ToString();
            var orderNumber = NextOrderNumber();

            if (IsValid(order))
            {
                var orderAccepted = new OrderAccepted(Guid.Empty.ToString())
                {
                    OrderId = orderId,
                    OrderNumber = orderNumber,
                    Paste = order.Paste,
                    Tomatoes = order.Tomatoes,
                    Cheese = order.Cheese,
                    Amount = order.Amount,
                    CustomerId = order.CustomerId
                };

                // Put the Event into EventStream.
                // EntityId is needed because the Stream doesn't know the Id and can't read from Event 
                await eventStream.Append(orderAccepted, orderAccepted.OrderId);
                
                return new OkObjectResult(new
                {
                    orderNumber = orderAccepted.OrderNumber,
                    orderId = orderAccepted.OrderId,
                    message = "Order accepted"
                });
            }
            else
            {
                var orderDeclined = new OrderDeclined(Guid.Empty.ToString())
                {
                    OrderId = orderId,
                    OrderNumber = orderNumber,
                    OrderContent =
                        $"Paste: {order.Paste}, Tomatoes: {order.Tomatoes}, Cheese{order.Cheese}, Amount: {order.Amount}",
                    CustomerId = order.CustomerId,
                    Reason = "[put the reason in here]"
                };

                try
                {
                    
                }
                catch (Exception ex)
                {
                    log.LogError(ex.Message, ex);
                    
                    return new BadRequestObjectResult(new
                    {
                        errorCode = "ORDER_DECLINED_BUT_SERVER_ERROR",
                        message = $"Order declined because of [put the reason in here]" + 
                                  $"(Internal Server Error: {ex.Message}"
                    });
                }

                return new BadRequestObjectResult(new
                {
                    errorCode = "ORDER_DECLINED",
                    message = "Order declined because of [put the reason in here]"
                });
            }
        }

        private static bool IsValid(Order order)
        {
            return string.IsNullOrWhiteSpace(order.Paste) == false;
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