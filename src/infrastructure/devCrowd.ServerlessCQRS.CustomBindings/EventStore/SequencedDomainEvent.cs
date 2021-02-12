namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    public class SequencedDomainEvent
    {
        public SequencedDomainEvent(long sequenceNumber, object instance)
        {
            SequenceNumber = sequenceNumber;
            Instance = instance;
        }
        
        public long SequenceNumber { get; }
        public object Instance { get; }
    }
}