using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Bluefragments.Utilities.Data.Cosmos.Tests
{
    /// <summary>
    /// BEWARE ! The tests clean up after them selfves. Run these against a DEV (preferably localhost) database.
    /// </summary>
    public class CosmosClientTests : IAsyncLifetime
    {
        private readonly string testCollection;
        private readonly CosmosClient<DataEntityBase<string>, string> cosmosClient;

        public CosmosClientTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var database = configuration["Database"] ?? throw new ArgumentNullException("Database");
            var key = configuration["Key"] ?? throw new ArgumentNullException("Key");
            var url = configuration["Uri"] ?? throw new ArgumentNullException("Uri");
            testCollection = configuration["Collection"] ?? throw new ArgumentNullException("Collection");

            cosmosClient = new CosmosClient<DataEntityBase<string>, string>(database, key, url);
        }

        public Task InitializeAsync()
        {
            return Task.FromResult<object>(null);
        }

        public async Task DisposeAsync()
        {
            await cosmosClient.DeleteContainerAsync(testCollection);
        }

        [Fact]
        public async Task UpsertItemAsync_ItemIsCreated()
        {
            var itemText = "Hello";
            var item = new TestEntity()
            {
                Text = itemText,
                Id = Guid.NewGuid().ToString(),
            };

            try
            {
                var id = await cosmosClient.UpsertItemAsync(item, testCollection);

                Assert.NotNull(id);
                Assert.NotEmpty(id);

                var resultItem = await cosmosClient.GetItemAsync<TestEntity>(id, item.Type, testCollection);

                Assert.NotNull(resultItem);
                Assert.Equal(resultItem.Text, itemText);
            }
            finally
            {
                await cosmosClient.DeleteItemAsync(item.Id, testCollection, item.Type);
            }
        }

        [Fact]
        public async Task UpsertItemAsync_WrongETag_ThrowsConcurrencyException()
        {
            var itemText = "Hello";
            var item = new TestEntity()
            {
                Text = itemText,
                Id = Guid.NewGuid().ToString(),
            };

            try
            {
                var id = await cosmosClient.UpsertItemAsync(item, testCollection);

                Assert.NotNull(id);
                Assert.NotEmpty(id);

                await Assert.ThrowsAsync<ConcurrencyException>(() => cosmosClient.UpsertItemAsync(item, testCollection, "Hello"));

                var resultItem = await cosmosClient.GetItemAsync<TestEntity>(id, item.Type, testCollection);

                Assert.NotNull(resultItem);
                Assert.Equal(resultItem.Text, itemText);
            }
            finally
            {
                await cosmosClient.DeleteItemAsync(item.Id, testCollection, item.Type);
            }
        }

        [Fact]
        public async Task UpsertItemAsync_ETagMatches_Succeeds()
        {
            var itemText = "World";
            var item = new TestEntity()
            {
                Text = "Hello",
                Id = Guid.NewGuid().ToString(),
            };

            try
            {
                var id = await cosmosClient.UpsertItemAsync(item, testCollection);

                Assert.NotNull(id);
                Assert.NotEmpty(id);

                var itemFromDb = await cosmosClient.GetItemAsync<TestEntity>(id, item.Type, testCollection);
                itemFromDb.Text = itemText;

                await cosmosClient.UpsertItemAsync(itemFromDb, testCollection, itemFromDb.Etag);

                var resultItem = await cosmosClient.GetItemAsync<TestEntity>(id, item.Type, testCollection);

                Assert.NotNull(resultItem);
                Assert.Equal(resultItem.Text, itemText);
            }
            finally
            {
                await cosmosClient.DeleteItemAsync(item.Id, testCollection, item.Type);
            }
        }

        [Fact]
        public async Task UpdateItemAsync_ItemIsUpdated()
        {
            var item = new TestEntity()
            {
                Text = "Hello",
                Id = Guid.NewGuid().ToString(),
            };

            try
            {
                var id = await cosmosClient.UpsertItemAsync(item, testCollection);

                Assert.NotNull(id);
                Assert.NotEmpty(id);

                var itemText = "Test";
                item.Text = itemText;
                await cosmosClient.UpdateItemAsync(item, testCollection);

                var resultItem = await cosmosClient.GetItemAsync<TestEntity>(id, item.Type, testCollection);

                Assert.NotNull(resultItem);
                Assert.Equal(resultItem.Text, itemText);
                Assert.Equal(resultItem.Id, item.Id);
            }
            finally
            {
                await cosmosClient.DeleteItemAsync(item.Id, testCollection, item.Type);
            }
        }

        [Fact]
        public async Task DeleteItemAsync_ItemExists_ItemIsDeleted()
        {
            var item = new TestEntity()
            {
                Text = "Hello",
                Id = Guid.NewGuid().ToString(),
            };

            var id = await cosmosClient.UpsertItemAsync(item, testCollection);

            Assert.NotNull(id);
            Assert.NotEmpty(id);

            await cosmosClient.DeleteItemAsync(item.Id, testCollection, item.Type);
            await Assert.ThrowsAsync<CosmosException>(async () => await cosmosClient.GetItemAsync<TestEntity>(id, item.Type, testCollection));
        }

        [Fact]
        public async Task GetItemsAsync_ItemsExists_ReturnsAllItems()
        {
            for (int i = 0; i < 2; i++)
            {
                var toCreate = new TestEntity()
                {
                    Text = "Hello",
                    Id = i.ToString(),
                };

                _ = await cosmosClient.UpsertItemAsync(toCreate, testCollection);
            }

            var allItems = await cosmosClient.GetItemsAsync<TestEntity>(testCollection);
            Assert.Equal(2, allItems.Count());

            for (int i = 0; i < 2; i++)
            {
                await cosmosClient.DeleteItemAsync(i.ToString(), testCollection, TestEntity.TypeName);
            }
        }

        [Fact]
        public async Task GetItemsAsync_ItemsExists_FiltersItems()
        {
            for (int i = 0; i < 2; i++)
            {
                var item = new TestEntity()
                {
                    Text = "Hello",
                    Id = i.ToString(),
                };

                _ = await cosmosClient.UpsertItemAsync(item, testCollection);
            }

            var allItems = await cosmosClient.GetItemsAsync<TestEntity>(x => x.Id == "1", testCollection);
            Assert.Single(allItems);

            for (int i = 0; i < 2; i++)
            {
                await cosmosClient.DeleteItemAsync(i.ToString(), testCollection, TestEntity.TypeName);
            }
        }

        [Fact]
        public async Task GetItemAsync_ItemExists_FiltersItems()
        {
            for (int i = 0; i < 2; i++)
            {
                var toCreate = new TestEntity()
                {
                    Text = "Hello",
                    Id = i.ToString(),
                };

                _ = await cosmosClient.UpsertItemAsync(toCreate, testCollection);
            }

            var expectedId = "1";
            var item = await cosmosClient.GetItemAsync<TestEntity>(x => x.Id == expectedId, testCollection);
            Assert.NotNull(item);
            Assert.Equal(expectedId, item.Id);

            for (int i = 0; i < 2; i++)
            {
                await cosmosClient.DeleteItemAsync(i.ToString(), testCollection, TestEntity.TypeName);
            }
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public async Task GetFirstAsync_ItemExists_FindsFirst(bool useOrderByDescending, int expectedOrder)
        {
            for (int i = 0; i < 2; i++)
            {
                var toCreate = new TestEntity()
                {
                    Text = "Hello",
                    Id = i.ToString(),
                    Order = i,
                };

                _ = await cosmosClient.UpsertItemAsync(toCreate, testCollection);
            }

            var item = await cosmosClient.GetFirstAsync<TestEntity>(
                x => true,
                useOrderByDescending,
                x => x.Order,
                testCollection);

            Assert.NotNull(item);
            Assert.Equal(expectedOrder, item.Order);

            for (int i = 0; i < 2; i++)
            {
                await cosmosClient.DeleteItemAsync(i.ToString(), testCollection, TestEntity.TypeName);
            }
        }

        [Fact]
        public async Task GetFirstItemsAsync_ItemsExists_FindsCorrectNumberOfItems()
        {
            for (int i = 0; i < 5; i++)
            {
                var toCreate = new TestEntity()
                {
                    Text = "Hello",
                    Id = i.ToString(),
                    Order = i,
                };

                _ = await cosmosClient.UpsertItemAsync(toCreate, testCollection);
            }

            var result = await cosmosClient.GetFirstItemsAsync<TestEntity>(
                x => true,
                true,
                x => x.Order,
                3,
                testCollection);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());

            for (int i = 0; i < 5; i++)
            {
                await cosmosClient.DeleteItemAsync(i.ToString(), testCollection, TestEntity.TypeName);
            }
        }

        [Fact]
        public async Task GetFirstItemsAsync_ExceedsNumberOfItems_DoesNotFail()
        {
            for (int i = 0; i < 2; i++)
            {
                var toCreate = new TestEntity()
                {
                    Text = "Hello",
                    Id = i.ToString(),
                    Order = i,
                };

                _ = await cosmosClient.UpsertItemAsync(toCreate, testCollection);
            }

            var result = await cosmosClient.GetFirstItemsAsync<TestEntity>(
                x => true,
                true,
                x => x.Order,
                3,
                testCollection);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            for (int i = 0; i < 2; i++)
            {
                await cosmosClient.DeleteItemAsync(i.ToString(), testCollection, TestEntity.TypeName);
            }
        }

        private class TestEntity : DataEntityBase<string>
        {
            public const string TypeName = "testEntity";

            public TestEntity()
            {
                Type = TypeName;
            }

            public string Text { get; set; }

            public int Order { get; set; }

            [JsonProperty("_etag")]
            public string Etag { get; set; }
        }
    }
}
