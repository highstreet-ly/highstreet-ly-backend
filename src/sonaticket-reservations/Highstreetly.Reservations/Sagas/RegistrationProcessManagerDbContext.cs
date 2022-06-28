using Highstreetly.Infrastructure.Processors;
using Microsoft.EntityFrameworkCore;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerDbContext : DbContext
    {
        private readonly string _connectionString;
        public const string SchemaName = "TicketingEventRegistrationProcesses";

        public RegistrationProcessManagerDbContext(DbContextOptions<RegistrationProcessManagerDbContext> opts)
            : base(opts)
        {
        }

        public RegistrationProcessManagerDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RegistrationProcessManager>().ToTable("RegistrationProcess", SchemaName).HasKey("Id");
            modelBuilder.Entity<UndispatchedMessages>().ToTable("UndispatchedMessages", SchemaName);
        }

        // Define the available entity sets for the database.
        public DbSet<RegistrationProcessManager> RegistrationProcesses { get; set; }

        // Table for pending undispatched messages associated with a process manager.
        public DbSet<UndispatchedMessages> UndispatchedMessages { get; set; }
    }
}