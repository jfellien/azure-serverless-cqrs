using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace devCrowd.ServerlessCQRS.Notifications
{
    public static class Negotiate
    {
        [FunctionName("Negotiate")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "negotiate/{clientId}")]
            HttpRequest req,
            string clientId,
            [SignalRConnectionInfo(HubName = "%COMMUNICATION_HUB_NAME%", UserId = "{clientId}")]
            SignalRConnectionInfo communicationConnectionInfo,
            ILogger log)
        {
            if (string.IsNullOrWhiteSpace(clientId) == false)
            {
                log.LogInformation($"Negotiate Communication with Client {clientId}");
                
                return new OkObjectResult(new
                {
                    url = communicationConnectionInfo.Url,
                    accessToken = communicationConnectionInfo.AccessToken,
                    message = "You need for receiving messages a few target event names. Take a look into list.",
                    targets = new {
                        targetInfo = Environment.GetEnvironmentVariable("TARGET_INFO"),
                        targetWarning = Environment.GetEnvironmentVariable("TARGET_WARNING"),
                        targetError = Environment.GetEnvironmentVariable("TARGET_ERROR")
                    }
                });
            }

            return new BadRequestObjectResult("Missing clientId");
        }
    }
}