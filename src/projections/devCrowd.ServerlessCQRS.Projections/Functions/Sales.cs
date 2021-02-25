using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Core.Projections.Sales;
using devCrowd.ServerlessCQRS.ProjectionsStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace devCrowd.ServerlessCQRS.Projections.Functions
{
    public class Sales
    {
        private readonly IStoreProjections _projectionsStorage;

        public Sales(IStoreProjections projectionsStorage)
        {
            _projectionsStorage = projectionsStorage;
        }
        
        [FunctionName("GetAcceptedOrders")]
        public async Task<IActionResult> GetAcceptedOrders(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "accepted-orders")]
            HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult(await _projectionsStorage.GetAll<AcceptedOrder>());
        }
        
        [FunctionName("GetAcceptedOrder")]
        public async Task<IActionResult> GetAcceptedOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "accepted-orders/{id}")]
            HttpRequest req,
            string id,
            ILogger log)
        {
            return new OkObjectResult(await _projectionsStorage.Get<AcceptedOrder>(id));
        }
        
        [FunctionName("GetOrderItems")]
        public async Task<IActionResult> GetOrderItems(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "order-items")]
            HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult(await _projectionsStorage.GetAll<OrderItem>());
        }
    }
}