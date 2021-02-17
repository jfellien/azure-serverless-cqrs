using System;

namespace devCrowd.ServerlessCQRS.Infrastructure.Lib.Notifications
{
    public class BroadcastMessage : IBroadcastMessage
    {
        public BroadcastMessage()
        {
            UtcTimestamp = DateTime.UtcNow.ToString("O");
        }
        public string UtcTimestamp { get; set; }
        public string From { get; set; }
        public string MessageText { get; set; }
    }
}