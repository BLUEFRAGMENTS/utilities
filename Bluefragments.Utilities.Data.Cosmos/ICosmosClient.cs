using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public interface ICosmosClient<TY>
    {
        Task<T> GetFirstAsync<T>(Expression<Func<T, bool>> whereFunction, bool useOrderByDescending, Expression<Func<T, long>> orderByFunction, string collection)
            where T : TY;

        Task DeleteItemAsync<T>(string id, string collection, string partitionKey);

        Task<IEnumerable<T>> GetItemsAsync<T>(string collection)
            where T : TY;

        Task<IEnumerable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate, string collection)
            where T : TY;

        Task<IEnumerable<dynamic>> GetItemsAsync(string collection, string query);

        Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate, string collection)
            where T : TY;

        Task<T> GetItemAsync<T>(object id, string collection)
            where T : TY;

        Task<object> UpsertItemAsync<T>(T item, string collection)
            where T : TY;

        Task<object> UpdateItemAsync<T>(T item, string collection)
            where T : TY;

        Task<BulkOperationResponse<T>> UpsertConcurrentlyAsync<T>(Container container, IReadOnlyList<T> documentsToWorkWith)
            where T : TY;

        Task<BulkOperationResponse<T>> CreateConcurrentlyAsync<T>(Container container, IReadOnlyList<T> documentsToWorkWith)
            where T : TY;

        Task<BulkOperationResponse<T>> DeleteConcurrentlyAsync<T>(Container container, IReadOnlyList<T> documentsToWorkWith)
            where T : TY;
    }
}