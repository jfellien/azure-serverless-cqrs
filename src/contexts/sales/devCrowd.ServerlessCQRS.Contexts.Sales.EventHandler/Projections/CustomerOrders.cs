using System.Collections.Generic;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Projections
{
    internal class CustomerOrders : SalesProjection
    {
        public CustomerOrders(string id) : base(id)
        {
        }

        public List<SimplifiedOrder> AcceptedOrders { get; set; }
        public List<SimplifiedOrder> DeclinedOrders { get; set; }
    }
}