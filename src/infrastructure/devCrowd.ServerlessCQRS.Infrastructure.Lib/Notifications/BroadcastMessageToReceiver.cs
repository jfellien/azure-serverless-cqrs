namespace devCrowd.ServerlessCQRS.Infrastructure.Lib.Notifications
{
    public class BroadcastMessageToReceiver : BroadcastMessage
    {
        public string To { get; set; }
    }
}