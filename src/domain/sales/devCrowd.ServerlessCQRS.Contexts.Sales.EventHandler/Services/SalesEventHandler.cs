using System;
using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Core.Events.Sales;
using devCrowd.ServerlessCQRS.Core.Projections.Sales;
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

        public SalesEventHandler(IStoreProjections projectionsStore, ILogger<SalesEventHandler> log)
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

            await StoreCustomerOrder(orderDate, domainEvent);

            await StoreAcceptedOrder(orderDate, domainEvent);

            await StoreOrderItem(orderDate, domainEvent);
        }
        
        public async Task Handle(OrderDeclined domainEvent)
        {
            var orderDate = DateTime.Now.ToString("yy-MM-dd");
            
            var customerOrders = await _projectionsStore.Get<CustomerOrders>(domainEvent.CustomerId);

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

        private async Task StoreCustomerOrder(string orderDate, OrderAccepted domainEvent)
        {
            var customerOrders = await _projectionsStore.Get<CustomerOrders>(domainEvent.CustomerId);

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
        }

        private async Task StoreAcceptedOrder(string orderDate, OrderAccepted domainEvent)
        {
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

        private async Task StoreOrderItem(string orderDate, OrderAccepted domainEvent)
        {
            var customer = await _projectionsStore.Get<Customer>(domainEvent.CustomerId);

            await _projectionsStore.Add(new OrderItem(domainEvent.OrderId)
            {
                OrderNumber = domainEvent.OrderNumber,
                OrderDate = orderDate,
                Amount = domainEvent.Amount,
                Toppings = $"Paste: {domainEvent.Paste}, Tomatoes: {domainEvent.Tomatoes}, Cheese: {domainEvent.Cheese}",
                CustomerName = $"{customer.LastName} {customer.FirstName}",
                CustomerAddress = $"{customer.Address}, {customer.ZipCode} {customer.City}"
            });
        }
    }
}