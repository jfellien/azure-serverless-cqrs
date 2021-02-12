namespace devCrowd.ServerlessCQRS.Core.Events.Sales
{
    public class OrderPlaced
    {
        public string EntityId { get; set; }
        public string OrderNumber { get; set; }
        public string Paste { get; set; }
        public string Tomatoes { get; set; }
        public string Cheese { get; set; }
        public int Amount { get; set; }
    }
}