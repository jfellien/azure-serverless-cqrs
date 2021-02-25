using Newtonsoft.Json;

namespace devCrowd.ServerlessCQRS.ProjectionsStorage
{
    public interface IProjection
    {
        [JsonProperty("id")]
        string Id { get; set; }
        [JsonProperty("partitionKey")]
        string PartitionKey { get; }
        [JsonProperty("documentType")]
        string DocumentType { get; }
    }
}