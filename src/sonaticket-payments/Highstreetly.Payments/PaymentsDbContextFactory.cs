using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Highstreetly.Payments
{
    public class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
    {
        public PaymentsDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile($"sharedsettings.json", true, false)
                .Build();

            var builder = new DbContextOptionsBuilder<PaymentsDbContext>();

            var connectionString = configuration.GetConnectionString("PaymentsConnection");

            builder.UseNpgsql(connectionString);

            return new PaymentsDbContext(builder.Options);
        }
    }
}
