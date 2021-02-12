using Newtonsoft.Json;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    internal class LastSequenceNumberQueryResult
    {
        [JsonProperty("maxSequenceNumber")] 
        public long MaxSequenceNumber { get; set; }
    }
}