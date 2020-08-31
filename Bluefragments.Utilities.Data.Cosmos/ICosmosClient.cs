using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public interface ICosmosClient<TBaseEntity, TId>
    {
        Task<TEntity> GetFirstAsync<TEntity>(
            Expression<Func<TEntity, bool>> whereFunction,
            bool useOrderByDescending,
            Expression<Func<TEntity, long>> orderByFunction,
            string collection)
            where TEntity : TBaseEntity;
        Task DeleteItemAsync(string id, string collection, string partitionKey);


        Task<IEnumerable<TEntity>> GetItemsAsync<TEntity>(string collection)
            where TEntity : TBaseEntity;

        Task<IEnumerable<TEntity>> GetItemsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, string collection)
            where TEntity : TBaseEntity;

        Task<IEnumerable<dynamic>> GetItemsAsync(string collection, string query);

        Task<TEntity> GetItemAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, string collection)
            where TEntity : TBaseEntity;

        Task<TEntity> GetItemAsync<TEntity>(TId id, string partitionKey, string collection)
            where TEntity : TBaseEntity;

        Task<TId> UpsertItemAsync<TEntity>(TEntity item, string collection)
            where TEntity : class, TBaseEntity;

        Task<TId> UpdateItemAsync<TEntity>(TEntity item, string collection)
            where TEntity : class, TBaseEntity;

        Task<BulkOperationResponse<TEntity>> UpsertConcurrentlyAsync<TEntity>(string collection, IReadOnlyList<TEntity> documentsToWorkWith)
            where TEntity : class, TBaseEntity;

        Task<BulkOperationResponse<TEntity>> CreateConcurrentlyAsync<TEntity>(string collection, IReadOnlyList<TEntity> documentsToWorkWith)
            where TEntity : class, TBaseEntity;

        Task<BulkOperationResponse<TEntity>> DeleteConcurrentlyAsync<TEntity>(string collection, IReadOnlyList<TEntity> documentsToWorkWith)
            where TEntity : class, TBaseEntity;
    }
}
