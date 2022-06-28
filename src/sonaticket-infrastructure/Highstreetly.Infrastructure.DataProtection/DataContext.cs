using Microsoft.EntityFrameworkCore;

namespace Highstreetly.Infrastructure.DataProtection
{
    internal class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> Options)
            : base(Options)
        {

        }

        public DbSet<KeyValuesCollection> KeyCollections { get; set; }
    }
}