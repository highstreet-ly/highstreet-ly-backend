using Highstreetly.Management.Resources;
using Microsoft.EntityFrameworkCore;

namespace Highstreetly.Management
{
    public class ManagementDbContext : DbContext
    {
        private readonly string _connectionString;

        public virtual DbSet<EventOrganiser> EventOrganisers { get; set; }

        public virtual DbSet<EventSeries> EventSeries { get; set; }

        public virtual DbSet<EventInstance> EventInstances { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderTicket> OrderTickets { get; set; }

        public virtual DbSet<OrderTicketDetails> OrderTicketDetails { get; set; }

        public virtual DbSet<ProductExtra> ProductExtras { get; set; }

        public virtual DbSet<ProductExtraGroup> ProductExtraGroup { get; set; }

        public virtual DbSet<TicketType> TicketTypes { get; set; }

        public virtual DbSet<DashboardStat> DashboardStats { get; set; }

        public virtual DbSet<OrdersByDay> OrdersByDay { get; set; }

        public virtual DbSet<RefundsByDay> RefundsByDay { get; set; }

        public virtual DbSet<TicketsSoldByDay> TicketsSoldByDay { get; set; }

        public virtual DbSet<RegisteredInterestByDay> RegisteredInterestByDay { get; set; }

        public virtual DbSet<TicketTypeConfiguration> TicketTypeConfigurations { get; set; }

        public virtual DbSet<Image> Images { get; set; }

        public virtual DbSet<Plan> Plans { get; set; }

        public virtual DbSet<Subscription> Subscriptions { get; set; }

        public virtual DbSet<SubscriptionAddOn> SubscriptionAddOns { get; set; }

        public virtual DbSet<AddOn> AddOns { get; set; }

        public virtual DbSet<PlanAddOn> PlanAddOns { get; set; }

        public virtual DbSet<Feature> Features { get; set; }

        public virtual DbSet<PlanFeature> PlanFeatures { get; set; }

        public virtual DbSet<PlanTicket> PlanTickets { get; set; }

        public virtual DbSet<AddOnFeature> AddOnFeatures { get; set; }

        public virtual DbSet<ProductCategory> ProductCategories { get; set; }

        public virtual DbSet<BusinessTypeFeatureTemplate> BusinessTypeFeatureTemplates { get; set; }

        public virtual DbSet<BusinessType> BusinessTypes { get; set; }

        public virtual DbSet<LogEntry> LogEntries { get; set; }

        public ManagementDbContext(DbContextOptions<ManagementDbContext> opts)
                        : base(opts)
        {
        }

        public ManagementDbContext(string connectionString)
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

            modelBuilder.Entity<EventSeries>()
                .HasOne(x => x.EventOrganiser)
                .WithMany(x => x.EventSeries)
                .HasForeignKey(x => x.EventOrganiserId);
            
            modelBuilder.Entity<BusinessTypeFeatureTemplateFeature>()
                        .HasKey(bc => new { bc.BusinessTypeFeatureTemplateId, bc.FeatureId });  
            modelBuilder.Entity<BusinessTypeFeatureTemplateFeature>()
                        .HasOne(bc => bc.BusinessTypeFeatureTemplate)
                        .WithMany(b => b.BusinessTypeFeatureTemplateFeatures)
                        .HasForeignKey(bc => bc.BusinessTypeFeatureTemplateId);  
            modelBuilder.Entity<BusinessTypeFeatureTemplateFeature>()
                        .HasOne(bc => bc.Feature)
                        .WithMany(c => c.BusinessTypeFeatureTemplateFeatures)
                        .HasForeignKey(bc => bc.FeatureId);

            modelBuilder.Entity<EventInstance>(b =>
            {
                b.HasKey(u => u.Id);

                b.HasOne(x => x.BusinessType)
                 .WithMany(x => x.EventInstances)
                 .HasForeignKey(x => x.BusinessTypeId);

                b.HasMany(e => e.EventInstanceFeatures)
                 .WithOne(e => e.EventInstance)
                 .HasForeignKey(ur => ur.EventInstanceId)
                 .IsRequired();
            });

            modelBuilder.Entity<Subscription>()
                .HasOne(x => x.Plan)
                .WithMany(x => x.Subscriptions)
                .HasForeignKey(x => x.PlanId);

            modelBuilder.Entity<Subscription>()
                .HasOne<EventOrganiser>(s => s.EventOrganiser)
                .WithMany(eo => eo.Subscriptions)
                .HasForeignKey(x => x.EventOrganiserId);

            modelBuilder.Entity<Plan>(b =>
            {
                b.HasKey(u => u.Id);

                // Each Plan can have many entries in the PlanAddOns join table
                b.HasMany(e => e.PlanAddOns)
                 .WithOne(e => e.Plan)
                 .HasForeignKey(ur => ur.PlanId)
                 .IsRequired();

                // Each Plan can have many entries in the PlanFeatures join table
                b.HasMany(e => e.PlanFeatures)
                 .WithOne(e => e.Plan)
                 .HasForeignKey(ur => ur.PlanId)
                 .IsRequired();

                b.HasMany(e => e.PlanTickets)
                 .WithOne(e => e.Plan)
                 .HasForeignKey(ur => ur.PlanId)
                 .IsRequired();
            });

            modelBuilder.Entity<Subscription>(b =>
            {
                b.HasKey(u => u.Id);

                // Each Plan can have many entries in the SubscriptionAddOns join table
                b.HasMany(e => e.SubscriptionAddOns)
                    .WithOne(e => e.Subscription)
                    .HasForeignKey(ur => ur.SubscriptionId)
                    .IsRequired();
            });

            modelBuilder.Entity<AddOn>(b =>
            {
                b.HasKey(u => u.Id);

                // Each AddOn can have many entries in the PlanAddOns join table
                b.HasMany(e => e.PlanAddOns)
                    .WithOne(e => e.AddOn)
                    .HasForeignKey(ur => ur.AddOnId)
                    .IsRequired();

                // Each AddOn can have many entries in the AddOnFeatures join table
                b.HasMany(e => e.AddOnFeatures)
                    .WithOne(e => e.AddOn)
                    .HasForeignKey(ur => ur.AddOnId)
                    .IsRequired();

                // Each AddOn can have many entries in the SubscriptionAddOns join table
                b.HasMany(e => e.SubscriptionAddOns)
                    .WithOne(e => e.AddOn)
                    .HasForeignKey(ur => ur.AddOnId)
                    .IsRequired();
            });
            
            modelBuilder.Entity<BusinessTypeFeatureTemplateFeature>(b =>
            {
                b.HasKey(r => new { r.BusinessTypeFeatureTemplateId, r.FeatureId });
            });
            modelBuilder.Entity<PlanAddOn>(b =>
            {
                b.HasKey(r => new { r.PlanId, r.AddOnId });
            });

            modelBuilder.Entity<AddOnFeature>(b =>
            {
                b.HasKey(r => new { r.FeatureId, r.AddOnId });
            });

            modelBuilder.Entity<PlanFeature>(b =>
            {
                b.HasKey(r => new { r.FeatureId, r.PlanId });
            });
            
            modelBuilder.Entity<EventInstanceFeature>(b =>
            {
                b.HasKey(r => new { r.FeatureId, r.EventInstanceId });
            });

            modelBuilder.Entity<PlanTicket>(b =>
            {
                b.HasKey(r => new { r.TicketTypeId, r.PlanId });
            });

            modelBuilder.Entity<SubscriptionAddOn>(b =>
            {
                b.HasKey(r => new { r.SubscriptionId, r.AddOnId });
            });

            // order to order-ticket
            modelBuilder.Entity<OrderTicket>()
                .HasOne(x => x.Order)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.OrderId);

            // order-ticket to order-ticket-details
            modelBuilder.Entity<OrderTicket>()
                .HasOne(x => x.TicketDetails)
                .WithOne(x => x.OrderTicket)
                .HasForeignKey<Resources.OrderTicketDetails>(x => x.OrderTicketId);

            //  order-ticket-details to product-extras
            modelBuilder.Entity<ProductExtra>()
                .HasOne(x => x.OrderTicketDetails)
                .WithMany(x => x.ProductExtras)
                .HasForeignKey(x => x.OrderTicketDetailsId);

            modelBuilder.Entity<ProductExtraGroup>()
                .HasOne(x => x.TicketTypeConfiguration)
                .WithMany(x => x.ProductExtraGroups)
                .HasForeignKey(x => x.TicketTypeConfigurationId);

            modelBuilder.Entity<ProductExtraGroup>()
                .HasOne(x => x.TicketType)
                .WithMany(x => x.ProductExtraGroups)
                .HasForeignKey(x => x.TicketTypeId);

            modelBuilder.Entity<Image>()
                .HasOne(x => x.EventOrganiser)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.EventOrganiserId);

            modelBuilder.Entity<Image>()
                .HasOne(x => x.TicketType)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.TicketTypeId);

            modelBuilder.Entity<Image>()
                .HasOne(x => x.TicketTypeConfiguration)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.TicketTypeConfigurationId);

            modelBuilder.Entity<Image>()
                .HasOne(x => x.EventInstance)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.EventInstanceId);

            modelBuilder.Entity<Image>()
                .HasOne(x => x.EventSeries)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.EventSeriesId);

            modelBuilder.Entity<Image>()
                .HasOne(x => x.ProductCategory)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ProductCategoryId);


            modelBuilder.Entity<LogEntry>(b =>
            {
                b.HasKey(r => new { r.RaiseDate, r.Level });
            });

        }
    }
}