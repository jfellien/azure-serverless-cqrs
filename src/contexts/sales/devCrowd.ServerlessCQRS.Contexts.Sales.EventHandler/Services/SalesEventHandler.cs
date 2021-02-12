using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Core.Events.Sales;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.Extensions;
using Microsoft.Extensions.Logging;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Services
{
    internal class SalesEventHandler : IHandleEvents, IHandleSalesEvents
    {
        private readonly ILogger _log;

        public SalesEventHandler(ILogger log)
        {
            _log = log;
        }
        public Task Handle(IDomainEvent domainEvent)
        {
            return domainEvent.HandleMeWith(this);
        }
        
        public Task Handle(OrderPlaced domainEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}