using System;

namespace devCrowd.ServerlessCQRS.Core.Projections.Sales
{
    public class AcceptedOrder : SalesProjection
    {
        public const string DOCUMENT_TYPE = "accepted-order";

        public AcceptedOrder() : base(Guid.NewGuid().ToString(), DOCUMENT_TYPE) { }
        public AcceptedOrder(string id) : base(id, DOCUMENT_TYPE) { }
        public string Paste { get; set; }
        public string Tomatoes { get; set; }
        public string Cheese { get; set; }
        public int Amount { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string CustomerId { get; set; }
    }
}