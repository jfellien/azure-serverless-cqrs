using System.Threading.Tasks;

namespace devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing
{
    public interface IHandleEvents
    {
        Task Handle(IDomainEvent domainEvent);
    }
}