using System.Threading.Tasks;
using devCrowd.CustomBindings.EventSourcing.EventStreamStorages;

namespace devCrowd.ServerlessCQRS.Core.EventHandling
{
    public interface IHandleEvents
    {
        Task Handle(IDomainEvent domainEvent);
    }
}