using devCrowd.CustomBindings.EventSourcing.EventStreamStorages;

namespace devCrowd.ServerlessCQRS.Core.Events.Sales
{
    public class AmountOfOrderChanged : DomainEvent
    {
        public AmountOfOrderChanged(string requesterId) : base(requesterId) { }

        public string OrderId { get; set; }

        public int PreviousAmount { get; set; }

        public int NewAmount { get; set; }
    }
}