using Newtonsoft.Json;

namespace devCrowd.ServerlessCQRS.ProjectionsStorage
{
    public interface IProjection
    {
        [JsonProperty("id")]
        string Id { get; }
        string PartitionKey { get; }
    }
}