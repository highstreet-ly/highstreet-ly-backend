using System.IO;
using System.Linq;
using Highstreetly.Management;
using Highstreetly.Management.Resources;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Migrate
{ 
    class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("sharedsettings.json", true, false)
            .Build();

        static void Main(string[] args)
        {

            var services = new ServiceCollection();
            services.AddDbContext<ManagementDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("TicketManagementConnection"));
                options.EnableSensitiveDataLogging();
            });

            var provider = services.BuildServiceProvider();
            var ctx = provider.GetRequiredService<ManagementDbContext>();

            var instances = ctx.EventInstances.Include(x=>x.TicketTypes).ToList();

            foreach (var eventInstance in instances)
            {
                var productConfigs = ctx.TicketTypeConfigurations.Where(x => x.EventInstanceId == eventInstance.Id);

                var defaultCategory = new ProductCategory
                {
                    Id = NewId.NextGuid(),
                    EventInstance = eventInstance,
                    Name = "default",
                    Enabled = true
                };

                foreach (var ticketTypeConfiguration in productConfigs)
                {
                    ticketTypeConfiguration.ProductCategoryId = defaultCategory.Id;
                }

                foreach (var eventInstanceTicketType in eventInstance.TicketTypes)
                {
                    eventInstanceTicketType.ProductCategoryId = defaultCategory.Id;
                }

                defaultCategory.TicketTypeConfigurations.AddRange(productConfigs);
                defaultCategory.TicketTypes.AddRange(eventInstance.TicketTypes);

                ctx.ProductCategories.Add(defaultCategory);
            }

            ctx.SaveChanges();
        }
    }
}