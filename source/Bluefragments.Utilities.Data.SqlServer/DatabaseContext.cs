using Microsoft.EntityFrameworkCore;

namespace Bluefragments.Utilities.Data.SqlServer
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
    }
}
