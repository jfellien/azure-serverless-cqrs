using System;
using System.Collections;
using System.Collections.Generic;
using devCrowd.ServerlessCQRS.ProjectionsStorage;

namespace devCrowd.ServerlessCQRS.Core.Projections.PartsWarehouse
{
    public class Pastes : IProjection
    {
        private const string PARTITION_KEY = "parts-warehouse";
        private const string DOCUMENT_TYPE = "pastes-parts";
        
        public Pastes()
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