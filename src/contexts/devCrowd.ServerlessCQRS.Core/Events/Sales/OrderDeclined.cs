using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;

namespace devCrowd.ServerlessCQRS.Core.Events.Sales
{
    public class OrderDeclined : IDomainEvent
    {
        public string CustomerId { get; set; }
        public string Reason { get; set; }
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderContent { get; set; }
    }
}