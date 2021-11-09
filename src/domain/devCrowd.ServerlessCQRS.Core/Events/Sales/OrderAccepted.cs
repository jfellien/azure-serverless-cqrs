
using devCrowd.CustomBindings.EventSourcing;
using devCrowd.CustomBindings.EventSourcing.EventStreamStorages;

namespace devCrowd.ServerlessCQRS.Core.Events.Sales
{
    public class OrderAccepted : DomainEvent
    {
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string Paste { get; set; }
        public string Tomatoes { get; set; }
        public string Cheese { get; set; }
        public int Amount { get; set; }
        public string CustomerId { get; set; }
        
        public OrderAccepted(string requesterId) : base(requesterId) { }
    }
}