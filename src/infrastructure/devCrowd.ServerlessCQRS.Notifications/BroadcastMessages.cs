using System;
using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.Notifications;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace devCrowd.ServerlessCQRS.Notifications
{
    public static class BroadcastMessages
    {
        [FunctionName("SendInfo")]
        public static Task SendInfo(
            [ServiceBusTrigger("%BROADCASTER_INFO_QUEUE%", Connection = "BROADCASTER_CONNECTION_STRING")]
            BroadcastMessageToReceiver infoMessage,
            [SignalR(HubName = "%COMMUNICATION_HUB_NAME%")]
            IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            var broadcastMessage = new SignalRMessage
            {
                Target = Environment.GetEnvironmentVariable("TARGET_INFO"),
                Arguments = new object[]
                {
                    new
                    {
                        utcTimestamp = infoMessage.UtcTimestamp,
                        from = infoMessage.From,
                        message = infoMessage.MessageText
                    }
                }
            };

            if (string.IsNullOrWhiteSpace(infoMessage.To) == false)
            {
                broadcastMessage.UserId = infoMessage.To;
            }
            
            return signalRMessages.AddAsync(broadcastMessage);
        }
        
        [FunctionName("SendWarning")]
        public static Task SendWarning(
            [ServiceBusTrigger("%BROADCASTER_WARNING_QUEUE%", Connection = "BROADCASTER_CONNECTION_STRING")]
            BroadcastMessageToReceiver warningMessage,
            [SignalR(HubName = "%COMMUNICATION_HUB_NAME%")]
            IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            var broadcastMessage = new SignalRMessage
            {
                Target = Environment.GetEnvironmentVariable("TARGET_WARNING"),
                Arguments = new object[]
                {
                    new
                    {
                        utcTimestamp = warningMessage.UtcTimestamp,
                        from = warningMessage.From,
                        message = warningMessage.MessageText
                    }
                }
            };

            if (string.IsNullOrWhiteSpace(warningMessage.To) == false)
            {
                broadcastMessage.UserId = warningMessage.To;
            }
            
            return signalRMessages.AddAsync(broadcastMessage);
        }
        
        [FunctionName("SendError")]
        public static Task SendError(
            [ServiceBusTrigger("%BROADCASTER_ERROR_QUEUE%", Connection = "BROADCASTER_CONNECTION_STRING")]
            ErrorMessageToReceiver errorMessage,
            [SignalR(HubName = "%COMMUNICATION_HUB_NAME%")]
            IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            var broadcastMessage = new SignalRMessage
            {
                Target = Environment.GetEnvironmentVariable("TARGET_ERROR"),
                Arguments = new object[]
                {
                    new
                    {
                        utcTimestamp = errorMessage.UtcTimestamp,
                        from = errorMessage.From,
                        message = errorMessage.MessageText
                    }
                }
            };

            if (string.IsNullOrWhiteSpace(errorMessage.To) == false)
            {
                broadcastMessage.UserId = errorMessage.To;
            }
            
            // If Exception is not null, do what ever you like ;)

            return signalRMessages.AddAsync(broadcastMessage);
        }
    }
}