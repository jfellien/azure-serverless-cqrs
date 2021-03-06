using System;
using System.Collections.Generic;
using devCrowd.ServerlessCQRS.ProjectionsStorage;

namespace devCrowd.ServerlessCQRS.Core.Projections.PartsWarehouse
{
    public class Tomatoes : IProjection
    {
        private const string PARTITION_KEY = "parts-warehouse";
        private const string DOCUMENT_TYPE = "tomatoes-parts";
        
        public Tomatoes()
        {
            PartitionKey = PARTITION_KEY;
            DocumentType = DOCUMENT_TYPE;
        }
        public string Id { get; set; }
        public string PartitionKey { get; }
        public string DocumentType { get; }
        
        public IEnumerable<string> Types { get; set; }
    }
}