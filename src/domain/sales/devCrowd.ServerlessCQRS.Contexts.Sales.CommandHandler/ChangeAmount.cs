using System;
using System.Threading.Tasks;
using devCrowd.CustomBindings.EventSourcing;
using devCrowd.CustomBindings.EventSourcing.Extensions;
using devCrowd.ServerlessCQRS.Contexts.Sales.CommandHandler.Models;
using devCrowd.ServerlessCQRS.Core.Events.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.CommandHandler
{
    public static class ChangeAmount
    {
        [FunctionName("ChangeAmountOfOrder")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "order/{orderId}")]
            Amount amount,
            string orderId,
            [DomainEventStream("Sales", "Order", "{orderId}")]
            DomainEventStream eventStream,
            ILogger log)
        {

            var events = await eventStream.Events();

            var orderAccepted = events.GetFirst<OrderAccepted>().Where(order => order.OrderId == orderId);

            if (orderAccepted == null)
            {
                return new NotFoundResult();
            }

            var amountOfOrderChanged = new AmountOfOrderChanged(Guid.Empty.ToString())
            {
                OrderId = orderId,
                PreviousAmount = orderAccepted.Amount,
                NewAmount = amount.Value
            };

            await eventStream.Append(amountOfOrderChanged, orderId);

            return new OkObjectResult(new
            {
                prevoiusAmount = amountOfOrderChanged.PreviousAmount,
                newAmount = amountOfOrderChanged.NewAmount,
                message = "Amount changed"
            });
        }
    }
}