using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bluefragments.Utilities.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public class Storage<Y> : IDocumentStorage<Y> where Y : DataEntity
    {
        private readonly string database;
        private readonly CosmosClient client;

        public Storage(string database, string storageKey, string storageUrl)
        {
            database.ThrowIfParameterIsNullOrWhiteSpace(nameof(database));
            storageKey.ThrowIfParameterIsNullOrWhiteSpace(nameof(storageKey));
            storageUrl.ThrowIfParameterIsNullOrWhiteSpace(nameof(storageUrl));

            this.database = database;
            var authorizationKey = storageKey;
            var endpointUrl = storageUrl;

            CosmosClientOptions options = new CosmosClientOptions() { AllowBulkExecution = true };
            client = new CosmosClient(endpointUrl, authorizationKey, options);
        }

        public async Task<T> GetFirstAsync<T>(Expression<Func<T, bool>> whereFunction, bool useOrderByDescending, Expression<Func<T, long>> orderByFunction, string collection) where T : Y
        {
            var container = await GetContainerAsync(collection);

            // LINQ query generation
            FeedIterator<T> setIterator = null;

            if (useOrderByDescending)
            {
                setIterator = container.GetItemLinqQueryable<T>()
                    .Where(basePredicate<T>())
                    .Where(whereFunction)
                                .OrderByDescending(orderByFunction)
                                .ToFeedIterator();
            }
            else
            {
                setIterator = container.GetItemLinqQueryable<T>()
                    .Where(basePredicate<T>())
                    .Where(whereFunction)
                                    .OrderBy(orderByFunction)
                                    .ToFeedIterator();
            }

            var result = await setIterator.ReadNextAsync();

            return result?.FirstOrDefault<T>();
        }
        
        public async Task<T> GetItemAsync<T>(string id, string collection) where T : Y
        {
            return await GetItemAsync<T>((i => i.Id == id), collection);
        }

        public async Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate, string collection) where T : Y
        {
            var container = await GetContainerAsync(collection);

            FeedIterator<T> setIterator = null;

            setIterator = container.GetItemLinqQueryable<T>()
                .Where(basePredicate<T>())  
                .Where(predicate)
                            .ToFeedIterator();

            var result = await setIterator.ReadNextAsync();

            return result?.FirstOrDefault<T>();
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(string collection) where T : Y
        {
            return await GetItemsAsync<T>((i => 1 == 1), collection);
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate, string collection) where T : Y
        {
            var container = await GetContainerAsync(collection);
            var setIterator = container.GetItemLinqQueryable<T>()
                .Where(basePredicate<T>())
                .Where(predicate)
                .ToFeedIterator();

            List<T> results = new List<T>();
            //Asynchronous query execution
            while (setIterator.HasMoreResults)
            {
                foreach (var item in await setIterator.ReadNextAsync())
                {
                    {
                        results.Add(item);
                    }
                }
            }

            return results;
        }

        private async Task<string> CreateItemAsync<T>(T item, string collection) where T : Y
        {
            var container = await GetContainerAsync(collection);
            item.Id = Guid.NewGuid().ToString();

            var response = await container.CreateItemAsync(item);
            var created = response;

            return created.Resource.Id;
        }

        public async Task<string> UpdateItemAsync<T>(T item, string collection) where T : Y
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                return await CreateItemAsync<T>(item, collection);
            }

            var container = await GetContainerAsync(collection);

            var result = await container.ReplaceItemAsync(item, item.Id);

            return result.Resource.Id;
        }

        public async Task DeleteItemAsync<T>(string id, string collection, string partitionKey)
        {
            var container = await GetContainerAsync(collection);

            var result = await container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        protected virtual Expression<Func<T, bool>> basePredicate<T>() where T : Y 
        {
            return (i => 1 == 1);
        }
        
        private async Task<Container> GetContainerAsync(string collection)
        {
            if (string.IsNullOrEmpty(database) ||
                string.IsNullOrEmpty(collection))
            {
                throw new Exception("database parameters not valid");
            }

            var dbResponse = await client.CreateDatabaseIfNotExistsAsync(database);

            return dbResponse.Database.GetContainer(collection);     
        }
    }
}

