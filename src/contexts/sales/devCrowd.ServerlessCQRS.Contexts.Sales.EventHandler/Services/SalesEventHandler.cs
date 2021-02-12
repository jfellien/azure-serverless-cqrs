using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Core.Events.Sales;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Services
{
    internal class SalesEventHandler : IHandleSalesEvents
    {
        public Task Handle(OrderPlaced domainEvent)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IHandleSalesEvents : IHandleEvent<OrderPlaced>{}
}