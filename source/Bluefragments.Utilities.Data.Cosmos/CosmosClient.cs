using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Bluefragments.Utilities.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public class CosmosClient<TBaseEntity, TId> : ICosmosClient<TBaseEntity, TId>
        where TBaseEntity : class, ICosmosEntityBase<TId>
        where TId : class
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

        public async Task<IQueryable<TEntity>> GetQueryableAsync<TEntity>(string collection)
        {
            var container = await GetContainerAsync(collection);
            return container.GetItemLinqQueryable<TEntity>();
        }

        public async Task<TEntity> GetFirstAsync<TEntity>(
            Expression<Func<TEntity, bool>> whereFunction,
            bool useOrderByDescending,
            Expression<Func<TEntity, long>> orderByFunction,
            string collection)
            where TEntity : TBaseEntity
        {
            var result = await GetFirstItemsAsync<TEntity>(whereFunction, useOrderByDescending, orderByFunction, 1, collection);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<TEntity>> GetFirstItemsAsync<TEntity>(
            Expression<Func<TEntity, bool>> whereFunction,
            bool useOrderByDescending,
            Expression<Func<TEntity, long>> orderByFunction,
            int count,
            string collection)
            where TEntity : TBaseEntity
        {
            var container = await GetContainerAsync(collection);

            var setIterator = container.GetItemLinqQueryable<TEntity>()
                .Where(BasePredicate<TEntity>())
                .Where(whereFunction);

            setIterator = useOrderByDescending
                ? setIterator.OrderByDescending(orderByFunction)
                : setIterator.OrderBy(orderByFunction);

            var result = await setIterator.ToFeedIterator().ReadNextAsync();
            return result.Take(count);
        }

        public async Task<TEntity> GetItemAsync<TEntity>(TId id, string partitionKey, string collection)
            where TEntity : TBaseEntity
        {
            var container = await GetContainerAsync(collection);
            return await container.ReadItemAsync<TEntity>(id.ToString(), new PartitionKey(partitionKey));
        }

        public async Task<TEntity> GetItemAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, string collection)
            where TEntity : TBaseEntity
        {
            var result = await GetItemsAsync<TEntity>(predicate, collection);
            if (result == null)
            {
                return default;
            }

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<TEntity>> GetItemsAsync<TEntity>(string collection)
            where TEntity : TBaseEntity
        {
            return await GetItemsAsync<TEntity>(i => true, collection);
        }

        public async Task<IEnumerable<TEntity>> GetItemsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, string collection)
            where TEntity : TBaseEntity
        {
            var container = await GetContainerAsync(collection);
            var setIterator = container.GetItemLinqQueryable<TEntity>()
                .Where(BasePredicate<TEntity>())
                .Where(predicate)
                .ToFeedIterator();

            var results = new List<TEntity>();

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

        public async Task<TId> UpdateItemAsync<TEntity>(TEntity item, string collection)
            where TEntity : class, TBaseEntity
        {
            var container = await GetContainerAsync(collection);
            var result = await container.ReplaceItemAsync(item, item.Id.ToString());
            return result.Resource.Id;
        }

        public async Task<TId> UpdateDynamicAsync(dynamic item, string collection)
        {
            var container = await GetContainerAsync(collection);
            var result = await container.ReplaceItemAsync(item, item.id.ToString());
            return result.Resource.Id;
        }

        public async Task<TId> UpsertItemAsync<TEntity>(TEntity item, string collection, string ifMatchEtag = "")
            where TEntity : class, TBaseEntity
        {
            var container = await GetContainerAsync(collection);
            var options = new ItemRequestOptions();

            try
            {
                if (string.IsNullOrEmpty(ifMatchEtag) == false)
                {
                    options.IfMatchEtag = ifMatchEtag;
                }

                var result = await container.UpsertItemAsync(item, requestOptions: options);
                if (result?.Resource != null)
                {
                    return result.Resource.Id;
                }
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.PreconditionFailed)
                {
                    throw new ConcurrencyException($"ETag {ifMatchEtag} doesn't match the newest version of the document's ETag", ex);
                }

                throw;
            }

            return default;
        }

        public async Task DeleteItemAsync(string id, string collection, string partitionKey)
        {
            var container = await GetContainerAsync(collection);
            _ = await container.DeleteItemAsync<TBaseEntity>(id, new PartitionKey(partitionKey));
        }

        public async Task<BulkOperationResponse<TEntity>> UpsertConcurrentlyAsync<TEntity>(string collection, IReadOnlyList<TEntity> documentsToWorkWith)
            where TEntity : class, TBaseEntity
        {
            var operations = new List<Task<OperationResponse<TEntity>>>(documentsToWorkWith.Count);

            var container = await GetContainerAsync(collection);

            var type = typeof(TEntity);
            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(PartitionKeyAttribute), false));

            var attributes = properties.Select(a => new { attr = (PartitionKeyAttribute[])a.GetCustomAttributes(typeof(PartitionKeyAttribute), false), property = a }).Where(a => a.attr.Any(pk => pk.IsPartitionKey)).ToList();

            foreach (var document in documentsToWorkWith)
            {
                var partitionKey = attributes.FirstOrDefault().property.GetValue(document).ToString();
                operations.Add(container.UpsertItemAsync(document, new PartitionKey(partitionKey)).CaptureOperationResponse(document));
            }

            return await ExecuteTasksAsync(operations);
        }

        public async Task<BulkOperationResponse<TEntity>> CreateConcurrentlyAsync<TEntity>(string collection, IReadOnlyList<TEntity> documentsToWorkWith)
            where TEntity : class, TBaseEntity
        {
            var operations = new List<Task<OperationResponse<TEntity>>>(documentsToWorkWith.Count);

            var container = await GetContainerAsync(collection);

            var type = typeof(TEntity);
            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(PartitionKeyAttribute), false));

            var attributes = properties.Select(a => new { attr = (PartitionKeyAttribute[])a.GetCustomAttributes(typeof(PartitionKeyAttribute), false), property = a }).Where(a => a.attr.Any(pk => pk.IsPartitionKey)).ToList();

            foreach (var document in documentsToWorkWith)
            {
                var partitionKey = attributes.FirstOrDefault().property.GetValue(document).ToString();
                operations.Add(container.CreateItemAsync(document, new PartitionKey(partitionKey)).CaptureOperationResponse(document));
            }

            return await ExecuteTasksAsync(operations);
        }

        public async Task<BulkOperationResponse<TEntity>> DeleteConcurrentlyAsync<TEntity>(string collection, IReadOnlyList<TEntity> documentsToWorkWith)
            where TEntity : class, TBaseEntity
        {
            var type = typeof(TEntity);

            var container = await GetContainerAsync(collection);

            var properties = type.GetProperties().Where(prop => prop.IsDefined(typeof(PartitionKeyAttribute), false));
            var attributes = properties.Select(a => new { attr = (PartitionKeyAttribute[])a.GetCustomAttributes(typeof(PartitionKeyAttribute), false), property = a }).Where(a => a.attr.Any(pk => pk.IsPartitionKey)).ToList();

            var operations = new List<Task<OperationResponse<TEntity>>>(documentsToWorkWith.Count);

            foreach (var document in documentsToWorkWith)
            {
                var partitionKey = attributes.FirstOrDefault().property.GetValue(document).ToString();
                operations.Add(container.DeleteItemAsync<TEntity>(document.Id.ToString(), new PartitionKey(partitionKey)).CaptureOperationResponse(document));
            }

            return await ExecuteTasksAsync(operations);
        }

        public async Task CreateContainerIfNotExistsAsync(string collection, string partitionKey, int throughPut = 400)
        {
            collection.ThrowIfParameterIsNullOrWhiteSpace(nameof(collection));
            partitionKey.ThrowIfParameterIsNullOrWhiteSpace(nameof(partitionKey));

            var database = await GetDatabaseAsync();
            await database.CreateContainerIfNotExistsAsync(
                collection,
                partitionKey,
                throughPut);
        }

        public async Task DeleteContainerAsync(string collection)
        {
            collection.ThrowIfParameterIsNullOrWhiteSpace(nameof(collection));

            var container = await GetContainerAsync(collection);
            await container.DeleteContainerAsync();
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

        protected virtual Expression<Func<TEntity, bool>> BasePredicate<TEntity>()
            where TEntity : TBaseEntity
        {
            return i => true;
        }

        protected async Task<Container> GetContainerAsync(string collection)
        {
            collection.ThrowIfParameterIsNullOrWhiteSpace(nameof(collection));

            var database = await GetDatabaseAsync();
            return database.GetContainer(collection);
        }

        protected async Task<Database> GetDatabaseAsync()
        {
            var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(database);
            return databaseResponse.Database;
        }

        private async Task<TId> CreateItemAsync(TBaseEntity item, string collection)
        {
            var container = await GetContainerAsync(collection);
            var response = await container.CreateItemAsync(item);

            if (response?.Resource != null)
            {
                return response.Resource.Id;
            }

            return default;
        }
    }
}
