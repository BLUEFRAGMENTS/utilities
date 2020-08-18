using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Bluefragments.Utilities.Data.SqlServer
{
    public interface ISqlClient
    {
        IDisposable CreateTransactionScope();

        DbSet<T> Set<T>()
            where T : SqlDataEntity;

        Task<List<T>> ExecuteQueryAsync<T>(string query, params object[] parameters)
            where T : SqlDataEntity;

        Task CreateItemAsync<T>(T entity)
            where T : SqlDataEntity;

        Task DeleteItemAsync<T>(string id)
            where T : SqlDataEntity;

        Task<T> GetItemAsync<T>(string id)
            where T : SqlDataEntity;

        Task<T> GetItemAsync<T>(Expression<Func<T, bool>> expression)
            where T : SqlDataEntity;

        Task<List<T>> GetItemsAsync<T>()
            where T : SqlDataEntity;

        Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> expression)
            where T : SqlDataEntity;

        Task ModifyItemAsync<T>(string id, Action<T> modifyItemAction)
            where T : SqlDataEntity;

        void UpdateItem<T>(T entity)
            where T : SqlDataEntity;
    }
}