using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;

namespace devCrowd.ServerlessCQRS.Core.Events.Sales
{
    public class OrderAccepted : IDomainEvent
    {
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string Paste { get; set; }
        public string Tomatoes { get; set; }
        public string Cheese { get; set; }
        public int Amount { get; set; }
        public string CustomerId { get; set; }
    }
}