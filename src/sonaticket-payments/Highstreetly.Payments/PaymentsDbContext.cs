
using Highstreetly.Payments.Contracts;
using Highstreetly.Payments.Domain;
using Highstreetly.Payments.Resources;
using Highstreetly.Payments.ViewModels.Payments;
using Microsoft.EntityFrameworkCore;

namespace Highstreetly.Payments
{
    public class PaymentsDbContext : DbContext
    {
        private readonly string _connectionString;
        public const string SchemaName = "TicketedEventPayments";

        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> opts)
            : base(opts)
        {
        }

        public PaymentsDbContext(string connectionString)
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

            modelBuilder.Entity<StripeCustomer>().ToTable("StripeCustomers", SchemaName);

            modelBuilder.Entity<ThirdPartyProcessorPayment>().ToTable("ThirdPartyProcessorPayments", SchemaName);
            modelBuilder.Entity<ThirdPartyProcessorPaymentItem>().ToTable("ThidPartyProcessorPaymentItems", SchemaName);

            modelBuilder.Entity<Payment>()
                        .HasMany(x => x.Charges)
                        .WithOne(x => x.Payment);

            modelBuilder.Entity<Charge>()
                        .HasMany(x => x.Refunds)
                        .WithOne(x => x.Charge);
            
            modelBuilder.Entity<LogEntry>(b =>
            {
                b.HasKey(r => new { r.RaiseDate, r.Level });
            });
            
            modelBuilder.Entity<Stats>()
                        .Property(x => x.Charged)
                        .HasColumnName("charged");
            
            modelBuilder.Entity<Stats>()
                        .Property(x => x.Id)
                        .HasColumnName("id");
            
            modelBuilder.Entity<Stats>()
                        .Property(x => x.Refunded)
                        .HasColumnName("refunded");

        }

        public DbSet<Charge> Charges { get; set; }

        public DbSet<RevenueByDay> RevenueByDay { get; set; }

        public DbSet<Stats> Stats { get; set; }
        public DbSet<Payment> Payments { get; set; }
        
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<ThirdPartyPayment> ThirdPartyPayments { get; set; }
        
        public DbSet<ThirdPartyProcessorPayment> ThirdPartyProcessorPayments { get; set; }
        
        public DbSet<StripeCustomer> StripeCustomers { get; set; }
        public DbSet<HsStripeEvent> HsStripeEvents { get; set; }
        
        public DbSet<LogEntry> LogEntries { get; set; }
    }
}