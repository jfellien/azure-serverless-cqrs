using System.Threading.Tasks;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    public interface IPublishDomainEvents
    {
        Task Publish(IDomainEvent domainEvent);
    }
}