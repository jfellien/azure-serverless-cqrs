namespace devCrowd.ServerlessCQRS.Infrastructure.Lib.Notifications
{
    public interface IBroadcastMessage
    {
        string UtcTimestamp { get; set; }
        string From { get; set; }
        string MessageText { get; set; }
    }
}