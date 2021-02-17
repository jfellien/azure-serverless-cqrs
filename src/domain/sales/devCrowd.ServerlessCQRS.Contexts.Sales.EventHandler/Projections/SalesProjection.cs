using devCrowd.ServerlessCQRS.ProjectionsStorage;

namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Projections
{
    internal class SalesProjection : IProjection
    {
        public const string PROJECTION_KEY = "sales";
        public SalesProjection(string id)
        {
            Id = id;
            PartitionKey = PROJECTION_KEY;
        }
        public string Id { get; }
        public string PartitionKey { get; }
    }
}