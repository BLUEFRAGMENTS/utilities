using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public interface IDocumentStorage<Y>
    {
        Task<T> GetFirstAsync<T>(Expression<Func<T, bool>> whereFunction, bool useOrderByDescending, Expression<Func<T, long>> orderByFunction, string collection) where T : Y;

        Task DeleteItemAsync<T>(string id, string collection, string partitionKey);

        Task<IEnumerable<T>> GetItemsAsync<T>(string collection) where T : Y;

        Task<IEnumerable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate, string collection) where T : Y;

        Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate, string collection) where T : Y;

        Task<T> GetItemAsync<T>(string id, string collection) where T : Y;

        Task<string> UpdateItemAsync<T>(T item, string collection) where T : Y;
    }
}