using System;
using System.IO;
using Highstreetly.Management;
using Highstreetly.Payments;
using Highstreetly.Permissions;
using Highstreetly.Reservations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace DeleteLogs
{
    class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                                               .SetBasePath(Directory.GetCurrentDirectory())
                                                               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                                                               .AddJsonFile("sharedsettings.json", true, false)
                                                               .Build();
        
        static void Main(string[] args)
        {
            Console.WriteLine("Cleaning down logs");

            var services = new ServiceCollection();
            
            Console.WriteLine(Configuration.GetConnectionString("TicketreservationsConnection"));
            Console.WriteLine(Configuration.GetConnectionString("TicketManagementConnection"));
            Console.WriteLine(Configuration.GetConnectionString("IdsConnection"));
            Console.WriteLine(Configuration.GetConnectionString("PaymentsConnection"));
            
            services.AddDbContext<ReservationDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("TicketreservationsConnection"));
                options.EnableSensitiveDataLogging();
            });
            
            services.AddDbContext<ManagementDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("TicketManagementConnection"));
                options.EnableSensitiveDataLogging();
            });
            
            services.AddDbContext<PermissionsDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("IdsConnection"));
                options.EnableSensitiveDataLogging();
            });
            
            services.AddDbContext<PaymentsDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("PaymentsConnection"));
                options.EnableSensitiveDataLogging();
            });

            var s = services.BuildServiceProvider();

            var reservations = s.GetService<ReservationDbContext>();
            reservations.Database.ExecuteSqlRaw("TRUNCATE logs");
            
            var mgmt = s.GetService<ManagementDbContext>();
            mgmt.Database.ExecuteSqlRaw("TRUNCATE logs");
            
            var payments = s.GetService<PaymentsDbContext>();
            payments.Database.ExecuteSqlRaw("TRUNCATE logs");
            
            var permissions = s.GetService<PermissionsDbContext>();
            permissions.Database.ExecuteSqlRaw("TRUNCATE logs");
            
            Console.WriteLine("Cleaning logs done");
        }
    }
}