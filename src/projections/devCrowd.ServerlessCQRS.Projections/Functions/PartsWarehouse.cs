using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Core.Projections.PartsWarehouse;
using devCrowd.ServerlessCQRS.ProjectionsStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace devCrowd.ServerlessCQRS.Projections.Functions
{
    public class PartsWarehouse
    {
        private readonly IStoreProjections _projectionsStore;

        public PartsWarehouse(IStoreProjections projectionsStore)
        {
            _projectionsStore = projectionsStore;
        }
        
        [FunctionName("GetParts")]
        public async Task<IActionResult> GetParts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "parts")]
            HttpRequest req, 
            ILogger log)
        {
            var pastes = _projectionsStore.GetAll<Pastes>();
            var tomatoes = _projectionsStore.GetAll<Tomatoes>();
            var cheeses = _projectionsStore.GetAll<Cheeses>();

            await Task.WhenAll(pastes, tomatoes, cheeses);

            return new OkObjectResult(new
            {
                pastes = pastes.Result.Count() == 1 ? pastes.Result.Single().Types : Array.Empty<string>(),
                tomatoes = tomatoes.Result.Count() == 1 ? tomatoes.Result.Single().Types : Array.Empty<string>(),
                cheeses = cheeses.Result.Count() == 1 ? cheeses.Result.Single().Types : Array.Empty<string>()
            });
        }
    }
}