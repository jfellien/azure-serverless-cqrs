using devCrowd.ServerlessCQRS.Core.EventHandling;
using devCrowd.ServerlessCQRS.Core.Events.Sales;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Services
{
    public interface IHandleSalesEvents : 
        IHandleEvent<OrderAccepted>,
        IHandleEvent<OrderDeclined>
    {}
}