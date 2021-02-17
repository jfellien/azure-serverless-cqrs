using System;
using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Projections;
using devCrowd.ServerlessCQRS.Core.Events.Sales;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.Extensions;
using devCrowd.ServerlessCQRS.ProjectionsStorage;
using Microsoft.Extensions.Logging;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Services
{
    internal class SalesEventHandler : IHandleEvents, IHandleSalesEvents
    {
        private readonly IStoreProjections _projectionsStore;
        private readonly ILogger _log;

        public SalesEventHandler(IStoreProjections projectionsStore, ILogger log)
        {
            _projectionsStore = projectionsStore;
            _log = log;
        }
        public Task Handle(IDomainEvent domainEvent)
        {
            _log.LogInformation($"Handle Event {domainEvent.GetType().Name}");
            
            return domainEvent.HandleMeWith(this);
        }
        public async Task Handle(OrderAccepted domainEvent)
        {
            var orderDate = DateTime.Now.ToString("yy-MM-dd");
            
            var customerOrders = await _projectionsStore.Get<CustomerOrders>(domainEvent.CustomerId, SalesProjection.PROJECTION_KEY);

            if (customerOrders != null)
            {
                customerOrders = new CustomerOrders(domainEvent.CustomerId);
            }
            
            customerOrders.AcceptedOrders.Add(new SimplifiedOrder
            {
                OrderId = domainEvent.OrderId,
                OrderNumber = domainEvent.OrderNumber,
                OrderDate = orderDate
            });

            await _projectionsStore.Replace(customerOrders);
            
            await _projectionsStore.Add(new AcceptedOrder(domainEvent.OrderId)
            {
                OrderDate = orderDate,
                OrderNumber = domainEvent.OrderNumber,
                Paste = domainEvent.Paste,
                Tomatoes = domainEvent.Tomatoes,
                Cheese = domainEvent.Cheese,
                Amount = domainEvent.Amount,
                CustomerId = domainEvent.CustomerId
            });
        }
        public async Task Handle(OrderDeclined domainEvent)
        {
            var orderDate = DateTime.Now.ToString("yy-MM-dd");
            
            var customerOrders = await _projectionsStore.Get<CustomerOrders>(domainEvent.CustomerId, SalesProjection.PROJECTION_KEY);

            if (customerOrders != null)
            {
                customerOrders = new CustomerOrders(domainEvent.CustomerId);
            }
            
            customerOrders.DeclinedOrders.Add(new SimplifiedOrder
            {
                OrderDate = orderDate,
                OrderId = domainEvent.OrderId,
                OrderNumber = domainEvent.OrderNumber
            });
        }
    }
}