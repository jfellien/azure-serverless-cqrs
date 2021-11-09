using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.Core.EventHandling
{
    public interface IHandleEvent<T>
    {
        Task Handle(T domainEvent);
    }
}