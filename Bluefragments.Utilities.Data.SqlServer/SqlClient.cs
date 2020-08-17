using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Bluefragments.Utilities.Data.SqlServer
{
    public class SqlClient : ISqlClient
    {
        private readonly DbContext context;

        public SqlClient(DbContext context)
        {
            this.context = context;
        }

        public IDisposable CreateTransactionScope()
        {
            return Disposable.Create(() =>
            {
                context.SaveChanges();
            });
        }

        public async Task CreateItemAsync<T>(T entity)
            where T : SqlDataEntity
        {
            await context.AddAsync(entity);
        }

        public async Task<T> GetItemAsync<T>(string id)
            where T : SqlDataEntity
        {
            return await context.FindAsync<T>(id);
        }

        public async Task<List<T>> GetItemsAsync<T>()
            where T : SqlDataEntity
        {
            var set = context.Set<T>();
            return await set.ToListAsync();
        }

        public async Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> expression)
            where T : SqlDataEntity
        {
            var set = context.Set<T>();
            return await set.Where(expression).ToListAsync();
        }

        public void UpdateItem<T>(T entity)
            where T : SqlDataEntity
        {
            context.Update(entity);
        }

        public async Task ModifyItemAsync<T>(string id, Action<T> modifyItemAction)
            where T : SqlDataEntity
        {
            var item = await context.FindAsync<T>(id);
            modifyItemAction?.Invoke(item);
        }

        public async Task DeleteItemAsync<T>(string id)
            where T : SqlDataEntity
        {
            var item = await context.FindAsync<T>(id);
            context.Remove(item);
        }
    }
}
