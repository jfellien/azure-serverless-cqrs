using System;
using System.Collections.Generic;

namespace devCrowd.ServerlessCQRS.Core.Projections.Sales
{
    public class CustomerOrders : SalesProjection
    {
        public const string DOCUMENT_TYPE = "customer-orders";

        public CustomerOrders() : base(Guid.NewGuid().ToString(), DOCUMENT_TYPE) { }
        public CustomerOrders(string id) : base(id, DOCUMENT_TYPE) { }
        public List<SimplifiedOrder> AcceptedOrders { get; set; }
        public List<SimplifiedOrder> DeclinedOrders { get; set; }
    }
}