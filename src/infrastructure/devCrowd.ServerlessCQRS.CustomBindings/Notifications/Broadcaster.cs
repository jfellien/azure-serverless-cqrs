using System;
using System.Text;
using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.Notifications;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace devCrowd.ServerlessCQRS.CustomBindings.Notifications
{
    public class Broadcaster
    {
        private readonly string _connectionString;
        private readonly string _infoQueueName;
        private readonly string _warningQueueName;
        private readonly string _errorQueueName;

        public Broadcaster(
            string connectionString, 
            string infoQueueName, 
            string warningQueueName, 
            string errorQueueName)
        {
            _connectionString = connectionString;
            _infoQueueName = infoQueueName;
            _warningQueueName = warningQueueName;
            _errorQueueName = errorQueueName;
        }

        public Task SendInfo(BroadcastMessage message)
        {
            var queueClient = GetInfoQueueClient();

            return Send(queueClient, message);
        }
        
        public Task SendInfo(BroadcastMessageToReceiver message)
        {
            var queueClient = GetInfoQueueClient();

            return Send(queueClient, message);
        }

        public Task SendWarning(BroadcastMessage message)
        {
            var queueClient = GetWarningQueueClient();

            return Send(queueClient, message);
        }
        
        public Task SendWarning(BroadcastMessageToReceiver message)
        {
            var queueClient = GetWarningQueueClient();

            return Send(queueClient, message);
        }

        public Task SendError(BroadcastMessage message)
        {
            var queueClient = GetErrorQueueClient();

            return Send(queueClient, message);
        }
        
        public Task SendError(BroadcastMessageToReceiver message)
        {
            var queueClient = GetErrorQueueClient();

            return Send(queueClient, message);
        }
        
        public Task SendError(ErrorMessage message)
        {
            var queueClient = GetErrorQueueClient();

            return Send(queueClient, message);
            
        }
        
        public Task SendError(ErrorMessageToReceiver message)
        {
            var queueClient = GetErrorQueueClient();

            return Send(queueClient, message);
        }

        private QueueClient GetInfoQueueClient()
        {
            return new QueueClient(_connectionString, _infoQueueName);
        }

        private QueueClient GetWarningQueueClient()
        {
            return new QueueClient(_connectionString, _warningQueueName);
        }

        private QueueClient GetErrorQueueClient()
        {
            return new QueueClient(_connectionString, _errorQueueName);
        }
        private Task Send(QueueClient queueClient, IBroadcastMessage message)
        {
            var broadcastMessageAsJson = JsonConvert.SerializeObject(message);
            var queueMessage = new Message(Encoding.UTF8.GetBytes(broadcastMessageAsJson));

            return queueClient.SendAsync(queueMessage);
        }
    }
}