using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public interface ICosmosClient<Y>
    {
        Task<T> GetFirstAsync<T>(Expression<Func<T, bool>> whereFunction, bool useOrderByDescending, Expression<Func<T, long>> orderByFunction, string collection) where T : Y;

        Task DeleteItemAsync<T>(string id, string collection, string partitionKey);

        Task<IEnumerable<T>> GetItemsAsync<T>(string collection) where T : Y;

        Task<IEnumerable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate, string collection) where T : Y;

        Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate, string collection) where T : Y;

        Task<T> GetItemAsync<T>(string id, string collection) where T : Y;

        Task<string> UpdateItemAsync<T>(T item, string collection) where T : Y;

        Task<BulkOperationResponse<T>> UpsertConcurrentlyAsync<T>(Container container, IReadOnlyList<T> documentsToWorkWith) where T : Y;

        Task<BulkOperationResponse<T>> CreateConcurrentlyAsync<T>(Container container, IReadOnlyList<T> documentsToWorkWith) where T : Y;

        Task<BulkOperationResponse<T>> DeleteConcurrentlyAsync<T>(string collection, IReadOnlyList<T> documentsToWorkWith) where T : Y;
    }
}