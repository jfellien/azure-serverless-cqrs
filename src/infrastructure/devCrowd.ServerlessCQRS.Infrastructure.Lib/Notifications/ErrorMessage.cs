using System;

namespace devCrowd.ServerlessCQRS.Infrastructure.Lib.Notifications
{
    public class ErrorMessage : BroadcastMessage
    {
        public Exception Exception { get; set; }
    }
}