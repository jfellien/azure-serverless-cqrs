using devCrowd.CustomBindings.EventSourcing.EventStreamStorages;

namespace devCrowd.ServerlessCQRS.Core.Events.Sales
{
    public class OrderDeclined : DomainEvent
    {
        public string CustomerId { get; set; }
        public string Reason { get; set; }
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderContent { get; set; }

        public OrderDeclined(string requesterId) : base(requesterId) { }
    }
}