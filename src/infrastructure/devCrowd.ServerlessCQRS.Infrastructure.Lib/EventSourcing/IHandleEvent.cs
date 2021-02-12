using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing
{
    public interface IHandleEvent<T>
    {
        Task Handle(T domainEvent);
    }
}