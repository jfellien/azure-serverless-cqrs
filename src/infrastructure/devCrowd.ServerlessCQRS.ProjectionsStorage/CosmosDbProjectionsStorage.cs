using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace devCrowd.ServerlessCQRS.ProjectionsStorage
{
    public class CosmosDbProjectionsStorage : IStoreProjections
    {
        private Container _projectionsContainer;

        public CosmosDbProjectionsStorage(string connectionString, string dbName, string collectionName)
        {
            _projectionsContainer = new CosmosClient(connectionString).GetContainer(dbName, collectionName);
        }

        public async Task<T> Get<T>(string id, string partitionKey)
        {
            var response = await _projectionsContainer
                .ReadItemAsync<T>(id, new PartitionKey(partitionKey));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException("Unable to get this projection");
            }

            return response.Resource;
        }

        public async Task Add<T>(T projection) where T : IProjection
        {
            var response = await _projectionsContainer
                .CreateItemAsync(projection, new PartitionKey(projection.PartitionKey));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException("Unable to add this projection");
            }
        }

        public async Task Replace<T>(T projection) where T : IProjection
        {
            var response = await _projectionsContainer
                .UpsertItemAsync(projection, new PartitionKey(projection.PartitionKey));
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException("Unable to replace this projection");
            }
        }

        public async Task Remove<T>(T projection) where T : IProjection
        {
            var response = await _projectionsContainer
                .DeleteItemAsync<T>(projection.Id, new PartitionKey(projection.PartitionKey));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException("Unable to remove this projection");
            }
        }
    }
}