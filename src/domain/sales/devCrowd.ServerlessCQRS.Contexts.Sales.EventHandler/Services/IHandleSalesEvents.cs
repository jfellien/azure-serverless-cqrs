using devCrowd.ServerlessCQRS.Core.Events.Sales;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Services
{
    public interface IHandleSalesEvents : 
        IHandleEvent<OrderAccepted>,
        IHandleEvent<OrderDeclined>
    {}
}