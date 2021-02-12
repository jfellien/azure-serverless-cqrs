using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Services
{
    public interface IHandleEvent<T>
    {
        Task Handle(T domainEvent);
    }
}