using Newtonsoft.Json;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    internal class EventStoreDomainEventWrap
    {
        [JsonProperty("id")]
        public string EventId { get; set; }
        [JsonProperty("eventName")]
        public string EventName { get; set; }
        [JsonProperty("eventFullName")]
        public string EventFullName { get; set; }
        [JsonProperty("isoTimestamp")]
        public string IsoTimestamp { get; set; }
        [JsonProperty("sequenceNumber")]
        public long SequenceNumber { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }
        [JsonProperty("entity")]
        public string Entity { get; set; }
        [JsonProperty("entityId")]
        public string EntityId { get; set; }
        
        [JsonProperty("payload")]
        public object DomainEvent { get; set; }
    }
}