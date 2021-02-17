namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Projections
{
    internal class SimplifiedOrder
    {
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
    }
}