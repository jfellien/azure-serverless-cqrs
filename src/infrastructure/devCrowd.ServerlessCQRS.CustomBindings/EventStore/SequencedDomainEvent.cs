using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    public class SequencedDomainEvent
    {
        public SequencedDomainEvent(long sequenceNumber, IDomainEvent instance)
        {
            SequenceNumber = sequenceNumber;
            Instance = instance;
        }
        
        public long SequenceNumber { get; }
        public IDomainEvent Instance { get; }
    }
}