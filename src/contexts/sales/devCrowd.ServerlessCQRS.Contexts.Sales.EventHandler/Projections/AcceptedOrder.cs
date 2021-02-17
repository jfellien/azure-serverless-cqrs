namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Projections
{
    internal class AcceptedOrder : SalesProjection
    {
        public AcceptedOrder(string id) : base(id) { }
        public string Paste { get; set; }
        public string Tomatoes { get; set; }
        public string Cheese { get; set; }
        public int Amount { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string CustomerId { get; set; }
    }
}