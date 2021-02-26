using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Extensions.Options;

namespace devCrowd.ServerlessCQRS.ProjectionsStorage
{
    public class CosmosDbProjectionsStorage : IStoreProjections
    {
        private Container _projectionsContainer;

        public CosmosDbProjectionsStorage(IOptions<ProjectionsStorageConfiguration> config)
        {
            _projectionsContainer = new CosmosClient(config.Value.ConnectionString)
                .GetContainer(config.Value.DatabaseName, config.Value.CollectionName);
        }

        public async Task<T> Get<T>(string id)  where T : IProjection, new()
        {
            var defaultTypeInstance = new T();

            using (var iterator = _projectionsContainer
                .GetItemLinqQueryable<T>()
                .Where(x => x.Id == id 
                            && x.DocumentType == defaultTypeInstance.DocumentType 
                            && x.PartitionKey == defaultTypeInstance.PartitionKey)
                .ToFeedIterator())
            {
                while (iterator.HasMoreResults)
                {
                    var itemsSequence = await iterator.ReadNextAsync();

                    if (itemsSequence.StatusCode != HttpStatusCode.OK)
                    {
                        throw new ApplicationException("Unable to get this projection");
                    }
                    
                    return itemsSequence.FirstOrDefault();
                }
            }

            return default;
        }
        
        public async Task<T> GetSingle<T>(Expression<Func<T, bool>> expression)  where T : IProjection, new()
        {
            var defaultTypeInstance = new T();
            


            using (var iterator = _projectionsContainer
                .GetItemLinqQueryable<T>()
                .Where(expression)
                .Where(x => x.DocumentType == defaultTypeInstance.DocumentType
                            && x.PartitionKey == defaultTypeInstance.PartitionKey)
                .ToFeedIterator())
            {
                while (iterator.HasMoreResults)
                {
                    var itemsSequence = await iterator.ReadNextAsync();

                    if (itemsSequence.StatusCode != HttpStatusCode.OK)
                    {
                        throw new ApplicationException("Unable to get this projection");
                    }

                    if (itemsSequence.Count > 1)
                    {
                        throw new ApplicationException("I've got more than one record");
                    }

                    if (itemsSequence.Count == 1)
                    {
                        return itemsSequence.First();
                    }
                }
            }

            return default;
        }
        public async Task<IEnumerable<T>> GetMany<T>(Expression<Func<T, bool>> expression)  where T : IProjection, new()
        {
            var defaultTypeInstance = new T();
            var manyItems = new List<T>();
            
            using (var iterator = _projectionsContainer
                .GetItemLinqQueryable<T>()
                .Where(expression)
                .ToFeedIterator())
            {
                while (iterator.HasMoreResults)
                {
                    var itemsSequence = await iterator.ReadNextAsync();

                    if (itemsSequence.StatusCode != HttpStatusCode.OK)
                    {
                        throw new ApplicationException("Unable to get this projection");
                    }
                    
                    manyItems.AddRange(itemsSequence);
                }
            }

            return manyItems;
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : IProjection, new()
        {
            var allItems = new List<T>();
            var defaultTypeInstance = new T();

            using (var iterator = _projectionsContainer
                .GetItemLinqQueryable<T>()
                .Where(x => x.DocumentType == defaultTypeInstance.DocumentType 
                            && x.PartitionKey == defaultTypeInstance.PartitionKey)
                .ToFeedIterator())
            {
                while (iterator.HasMoreResults)
                {
                    var itemsSequence = await iterator.ReadNextAsync();
                    
                    allItems.AddRange(itemsSequence);
                }
            }

            return allItems;
        }

        public async Task Add<T>(T projection) where T : IProjection
        {
            var response = await _projectionsContainer
                .CreateItemAsync(projection, new PartitionKey(projection.PartitionKey));

            if (response.StatusCode != HttpStatusCode.OK 
                && response.StatusCode != HttpStatusCode.Created)
            {
                throw new ApplicationException("Unable to add this projection");
            }
        }

        public async Task Replace<T>(T projection) where T : IProjection
        {
            var response = await _projectionsContainer
                .UpsertItemAsync(projection, new PartitionKey(projection.PartitionKey));
            
            if (response.StatusCode != HttpStatusCode.OK 
                && response.StatusCode != HttpStatusCode.Created)
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