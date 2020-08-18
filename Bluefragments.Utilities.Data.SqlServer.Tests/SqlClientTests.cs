using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Bluefragments.Utilities.Data.SqlServer.Tests
{
    public class SqlClientTests : IDisposable
    {
        private readonly SqlClient client;
        private readonly TestDatabaseContext context;

        public SqlClientTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDatabaseContext>()
                .UseSqlServer($"Server=(local); Database={Guid.NewGuid().ToString()}; Trusted_connection=true");

            context = new TestDatabaseContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            client = new SqlClient(context);
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task CreateItemAsync_ItemDoesNotExist_CreatesItem()
        {
            var tx = client.CreateTransactionScope();

            var entity = new TestEntity();
            entity.Text = "Hello";
            entity.Id = Guid.NewGuid().ToString();

            await client.CreateItemAsync(entity);
            tx.Dispose();
        }

        [Fact]
        public async Task GetItemAsync_ItemExists_ReturnsItem()
        {
            var id = Guid.NewGuid().ToString();
            using (var tx = client.CreateTransactionScope())
            {
                var entity = new TestEntity();
                entity.Text = "Hello";
                entity.Id = id;

                await client.CreateItemAsync(entity);
            }

            var result = await client.GetItemAsync<TestEntity>(id);

            Assert.NotNull(result);
            Assert.NotNull(result.Text);
        }

        [Fact]
        public async Task GetItemAsync_ItemDoesNotExists_ReturnsNull()
        {
            var id = Guid.NewGuid().ToString();
            var result = await client.GetItemAsync<TestEntity>(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateItemAsync_UpdatesItem()
        {
            var id = Guid.NewGuid().ToString();
            using (var tx = client.CreateTransactionScope())
            {
                var entity = new TestEntity();
                entity.Text = "Hello";
                entity.Id = id;

                await client.CreateItemAsync(entity);
            }

            var newText = "test";
            var item = await client.GetItemAsync<TestEntity>(id);
            item.Text = newText;

            using (var tx = client.CreateTransactionScope())
            {
                client.UpdateItem(item);
            }

            var result = await client.GetItemAsync<TestEntity>(id);
            Assert.Equal(newText, result.Text);
        }

        [Fact]
        public async Task DeleteItemAsync_DeletesItem()
        {
            var id = Guid.NewGuid().ToString();
            using (var tx = client.CreateTransactionScope())
            {
                var entity = new TestEntity();
                entity.Text = "Hello";
                entity.Id = id;

                await client.CreateItemAsync(entity);
            }

            using (var tx = client.CreateTransactionScope())
            {
                await client.DeleteItemAsync<TestEntity>(id);
            }

            var result = await client.GetItemAsync<TestEntity>(id);
            Assert.Null(result);
        }

        [Fact]
        public async Task ModifyItemAsync_ModifiesItem()
        {
            var id = Guid.NewGuid().ToString();
            using (var tx = client.CreateTransactionScope())
            {
                var entity = new TestEntity();
                entity.Text = "Hello";
                entity.Id = id;

                await client.CreateItemAsync(entity);
            }

            var newText = "Test";
            using (var tx = client.CreateTransactionScope())
            {
                await client.ModifyItemAsync<TestEntity>(id, item =>
                {
                    item.Text = newText;
                });
            }

            var result = await client.GetItemAsync<TestEntity>(id);
            Assert.NotNull(result);
            Assert.Equal(newText, result.Text);
        }

        [Fact]
        public async Task GetItems_ReturnsAllItems()
        {
            using (var tx = client.CreateTransactionScope())
            {
                for (int i = 0; i < 10; i++)
                {
                    var entity = new TestEntity();
                    entity.Text = i.ToString();
                    entity.Id = Guid.NewGuid().ToString();

                    await client.CreateItemAsync(entity);
                }
            }

            var result = await client.GetItemsAsync<TestEntity>();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(10, result.Count);
        }

        [Fact]
        public async Task GetItems_WithFilterExpression_ReturnsAppropriateItems()
        {
            using (var tx = client.CreateTransactionScope())
            {
                for (int i = 0; i < 10; i++)
                {
                    var entity = new TestEntity();
                    entity.Text = i.ToString();
                    entity.Id = Guid.NewGuid().ToString();

                    await client.CreateItemAsync(entity);
                }
            }

            var result = await client.GetItemsAsync<TestEntity>(x => x.Text == "1");

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task ExecuteQueryAsync_ReturnsAllItems()
        {
            using (var tx = client.CreateTransactionScope())
            {
                for (int i = 0; i < 10; i++)
                {
                    var entity = new TestEntity();
                    entity.Text = i.ToString();
                    entity.Id = Guid.NewGuid().ToString();

                    await client.CreateItemAsync(entity);
                }
            }

            var result = await client.ExecuteQueryAsync<TestEntity>("SELECT * FROM Tests");

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
