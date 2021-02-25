using devCrowd.ServerlessCQRS.ProjectionsStorage;

namespace devCrowd.ServerlessCQRS.Core.Projections.Sales
{
    public class SalesProjection : IProjection
    {
        private const string PARTITION_KEY = "sales";
        
        public SalesProjection(string id, string documentType)
        {
            Id = id;
            PartitionKey = PARTITION_KEY;
            DocumentType = documentType;
        }
        public string Id { get; set; }
        public string PartitionKey { get; }
        public string DocumentType { get; }
    }
}