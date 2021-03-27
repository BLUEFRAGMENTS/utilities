using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bluefragments.Utilities.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public class CosmosClient<TY> : ICosmosClient<TY>
        where TY : ICosmosEntity
    {
        private readonly string database;
        private readonly CosmosClient client;

        public CosmosClient(string database, string storageKey, string storageUrl)
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

        public async Task<T> GetFirstAsync<T>(Expression<Func<T, bool>> whereFunction, bool useOrderByDescending, Expression<Func<T, long>> orderByFunction, string collection)
            where T : TY
        {
            var container = await GetContainerAsync(collection);

            // LINQ query generation
            FeedIterator<T> setIterator = null;

            if (useOrderByDescending)
            {
                setIterator = container.GetItemLinqQueryable<T>()
                    .Where(BasePredicate<T>())
                    .Where(whereFunction)
                                .OrderByDescending(orderByFunction)
                                .ToFeedIterator();
            }
            else
            {
                setIterator = container.GetItemLinqQueryable<T>()
                    .Where(BasePredicate<T>())
                    .Where(whereFunction)
                                    .OrderBy(orderByFunction)
                                    .ToFeedIterator();
            }

            var result = await setIterator.ReadNextAsync();

            return result.FirstOrDefault<T>();
        }

        public async Task<T> GetItemAsync<T>(object id, string collection)
            where T : TY
        {
            return await GetItemAsync<T>(i => i.Id == id, collection);
        }

        public async Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate, string collection)
            where T : TY
        {
            var result = await GetItemsAsync<T>(predicate, collection);
            return result.FirstOrDefault<T>();
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(string collection)
            where T : TY
        {
            return await GetItemsAsync<T>(i => 1 == 1, collection);
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate, string collection)
            where T : TY
        {
            var container = await GetContainerAsync(collection);
            var setIterator = container.GetItemLinqQueryable<T>()
                .Where(BasePredicate<T>())
                .Where(predicate)
                .ToFeedIterator();

            List<T> results = new List<T>();

            // Asynchronous query execution
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

        public async Task<IEnumerable<dynamic>> GetItemsAsync(string collection, string query)
        {
            if (string.IsNullOrEmpty(collection))
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var container = await GetContainerAsync(collection);

            var setIterator = container.GetItemQueryIterator<dynamic>(query);

            var results = new List<dynamic>();
            while (setIterator.HasMoreResults)
            {
                foreach (var item in await setIterator.ReadNextAsync())
                {
                    results.Add(JsonConvert.SerializeObject(item));
                }
            }

            return results;
        }

        public async Task<object> UpdateItemAsync<T>(T item, string collection)
            where T : TY
        {
            if (string.IsNullOrEmpty(item.Id?.ToString()))
            {
                return await CreateItemAsync<T>(item, collection);
            }

            var container = await GetContainerAsync(collection);

            var result = await container.ReplaceItemAsync(item, item.Id.ToString());

            return result.Resource.Id;
        }

        public async Task<object> UpsertItemAsync<T>(T item, string collection)
            where T : TY
        {
            var container = await GetContainerAsync(collection);
            var result = await container.UpsertItemAsync(item);
            return result?.Resource?.Id;
        }

        public async Task DeleteItemAsync<T>(string id, string collection, string partitionKey)
        {
            var container = await GetContainerAsync(collection);

            var result = await container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        public async Task<BulkOperationResponse<T>> UpsertConcurrentlyAsync<T>(Container container, IReadOnlyList<T> documentsToWorkWith)
            where T : TY
        {
            List<Task<OperationResponse<T>>> operations = new List<Task<OperationResponse<T>>>(documentsToWorkWith.Count);

            Type type = typeof(T);
            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(PartitionKeyAttribute), false));

            var attributes = properties.Select(a => new { attr = (PartitionKeyAttribute[])a.GetCustomAttributes(typeof(PartitionKeyAttribute), false), property = a }).Where(a => a.attr.Any(pk => pk.IsPartitionKey)).ToList();

            foreach (var document in documentsToWorkWith)
            {
                var partitionKey = attributes.FirstOrDefault().property.GetValue(document).ToString();
                operations.Add(container.UpsertItemAsync(document, new PartitionKey(partitionKey)).CaptureOperationResponse(document));
            }

            return await ExecuteTasksAsync<T>(operations);
        }

        public async Task<BulkOperationResponse<T>> CreateConcurrentlyAsync<T>(Container container, IReadOnlyList<T> documentsToWorkWith)
            where T : TY
        {
            List<Task<OperationResponse<T>>> operations = new List<Task<OperationResponse<T>>>(documentsToWorkWith.Count);

            Type type = typeof(T);
            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(PartitionKeyAttribute), false));

            var attributes = properties.Select(a => new { attr = (PartitionKeyAttribute[])a.GetCustomAttributes(typeof(PartitionKeyAttribute), false), property = a }).Where(a => a.attr.Any(pk => pk.IsPartitionKey)).ToList();

            foreach (var document in documentsToWorkWith)
            {
                var partitionKey = attributes.FirstOrDefault().property.GetValue(document).ToString();
                operations.Add(container.CreateItemAsync(document, new PartitionKey(partitionKey)).CaptureOperationResponse(document));
            }

            return await ExecuteTasksAsync<T>(operations);
        }

        public async Task<BulkOperationResponse<T>> DeleteConcurrentlyAsync<T>(Container container, IReadOnlyList<T> documentsToWorkWith)
            where T : TY
        {
            Type type = typeof(T);
            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(PartitionKeyAttribute), false));
            var attributes = properties.Select(a => new { attr = (PartitionKeyAttribute[])a.GetCustomAttributes(typeof(PartitionKeyAttribute), false), property = a }).Where(a => a.attr.Any(pk => pk.IsPartitionKey)).ToList();

            List<Task<OperationResponse<T>>> operations = new List<Task<OperationResponse<T>>>(documentsToWorkWith.Count);

            foreach (var document in documentsToWorkWith)
            {
                var partitionKey = attributes.FirstOrDefault().property.GetValue(document).ToString();
                operations.Add(container.DeleteItemAsync<T>(document.Id.ToString(), new PartitionKey(partitionKey)).CaptureOperationResponse(document));
            }

            return await ExecuteTasksAsync<T>(operations);
        }

        protected async Task<BulkOperationResponse<T>> ExecuteTasksAsync<T>(IReadOnlyList<Task<OperationResponse<T>>> tasks)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            await Task.WhenAll(tasks);
            stopwatch.Stop();

            return new BulkOperationResponse<T>()
            {
                TotalTimeTaken = stopwatch.Elapsed,
                TotalRequestUnitsConsumed = tasks.Sum(task => task.Result.RequestUnitsConsumed),
                SuccessfullDocuments = tasks.Count(task => task.Result.IsSuccessfull),
                Failures = tasks.Where(task => !task.Result.IsSuccessfull).Select(task => (task.Result.Item, task.Result.CosmosException)).ToList(),
            };
        }

        protected virtual Expression<Func<T, bool>> BasePredicate<T>()
            where T : TY
        {
            return i => 1 == 1;
        }

        protected async Task<Container> GetContainerAsync(string collection)
        {
            if (string.IsNullOrEmpty(database) ||
                string.IsNullOrEmpty(collection))
            {
                throw new Exception("database parameters not valid");
            }

            var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(database);

            return databaseResponse.Database.GetContainer(collection);
        }

        private async Task<object> CreateItemAsync<T>(T item, string collection)
            where T : TY
        {
            var container = await GetContainerAsync(collection);
            item.Id = Guid.NewGuid().ToString();

            var response = await container.CreateItemAsync(item);
            var created = response;

            return created.Resource.Id;
        }
    }
}