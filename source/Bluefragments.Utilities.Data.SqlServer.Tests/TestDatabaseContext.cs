using Microsoft.EntityFrameworkCore;

namespace Bluefragments.Utilities.Data.SqlServer.Tests
{
    public class TestDatabaseContext : DbContext
    {
        public TestDatabaseContext()
        {
        }

        public TestDatabaseContext(DbContextOptions<TestDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<TestEntity> Tests { get; set; }
    }
}
