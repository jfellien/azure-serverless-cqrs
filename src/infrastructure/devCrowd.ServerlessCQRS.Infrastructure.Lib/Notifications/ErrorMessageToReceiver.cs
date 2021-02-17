using System;

namespace devCrowd.ServerlessCQRS.Infrastructure.Lib.Notifications
{
    public class ErrorMessageToReceiver : BroadcastMessageToReceiver
    {
        public Exception Exception { get; set; }
    }
}