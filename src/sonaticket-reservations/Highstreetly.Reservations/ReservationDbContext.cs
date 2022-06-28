using Highstreetly.Reservations.Resources;
using Microsoft.EntityFrameworkCore;

namespace Highstreetly.Reservations
{
    public class ReservationDbContext : DbContext
    {
        private readonly string _connectionString;

        public virtual DbSet<DraftOrder> DraftOrders { get; set; }
        
        public virtual DbSet<DraftOrderItem> DraftOrderItems { get; set; }
        
        public virtual DbSet<OrderTicketDetails> OrderTicketDetails { get; set; }
        
        public virtual DbSet<PricedOrder> PricedOrders { get; set; }
        
        public virtual DbSet<PricedOrderLine> PricedOrderLines { get; set; }

        public virtual DbSet<ProductExtra> ProductExtras { get; set; }

        public DbSet<LogEntry> LogEntries { get; set; }

        public ReservationDbContext(DbContextOptions<ReservationDbContext> opts)
            : base(opts)
        {
        }

        public ReservationDbContext(string connectionString)
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

            modelBuilder.Entity<PricedOrderLine>()
                        .HasOne(x => x.PricedOrder)
                        .WithMany(x => x.PricedOrderLines)
                        .HasForeignKey(x => x.PricedOrderId);

            modelBuilder.Entity<DraftOrderItem>()
                        .HasOne(x => x.DraftOrder)
                        .WithMany(x => x.DraftOrderItems)
                        .HasForeignKey(x => x.DraftOrderId);

            modelBuilder.Entity<DraftOrderItem>()
                        .HasOne(x => x.Ticket)
                        .WithOne(x => x.DraftOrderItem)
                        .HasForeignKey<Resources.OrderTicketDetails>(x => x.DraftOrderItemId);

            modelBuilder.Entity<ProductExtra>()
                        .HasOne(x => x.PricedOrderLine)
                        .WithMany(x => x.ProductExtras)
                        .HasForeignKey(x => x.PricedOrderLineId);

            modelBuilder.Entity<LogEntry>(b =>
            {
                b.HasKey(r => new { r.RaiseDate, r.Level });
            });
        }
    }
}