using System.IO;
using Highstreetly.Reservations.Sagas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Highstreetly.Reservations.Api
{
    public class RegistrationProcessManagerDbContextFactory : IDesignTimeDbContextFactory<RegistrationProcessManagerDbContext>
    {
        public RegistrationProcessManagerDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile($"sharedsettings.json", true, false)
                .Build();

            var builder = new DbContextOptionsBuilder<RegistrationProcessManagerDbContext>();

            var connectionString = configuration.GetConnectionString("TicketreservationsConnection");

            builder.UseNpgsql(connectionString, b => b.MigrationsAssembly("Sonatribe.Migrations"));

            return new RegistrationProcessManagerDbContext(builder.Options);
        }
    }
}