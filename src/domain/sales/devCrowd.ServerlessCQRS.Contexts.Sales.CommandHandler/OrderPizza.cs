using System;
using System.IO;
using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Contexts.Sales.CommandHandler.Models;
using devCrowd.ServerlessCQRS.Core.Events.Sales;
using devCrowd.ServerlessCQRS.CustomBindings.EventStore;
using devCrowd.ServerlessCQRS.CustomBindings.Notifications;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.Notifications;
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "order")]
            Order order,
            [DomainEventStream("Sales", "Order")]
            DomainEventStream eventStream,
            [Broadcaster] Broadcaster broadcaster,
            ILogger log)
        {
            /// Here is the place for business validation.
            /// If an Order is correct, we create the event OrderAccepted

            var orderId = Guid.NewGuid().ToString();
            var orderNumber = NextOrderNumber();

            if (IsValid(order))
            {
                var orderAccepted = new OrderAccepted
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

                await broadcaster.SendInfo(new BroadcastMessageToReceiver()
                {
                    From = "Pizza Factory - Order System",
                    To = order.RequestTraceId,
                    MessageText = $"Order accepted (Order Number: {orderNumber})"
                });

                return new OkObjectResult(new
                {
                    orderNumber = orderAccepted.OrderNumber,
                    orderId = orderAccepted.OrderId,
                    message = "Order accepted"
                });
            }
            else
            {
                var orderDeclined = new OrderDeclined
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
                    await broadcaster.SendWarning(new BroadcastMessageToReceiver()
                    {
                        From = "Pizza Factory - Order System",
                        To = order.RequestTraceId,
                        MessageText =
                            $"Order declined (Order Number: {orderNumber} ==> Reason : [pu the reason in here])"
                    });
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