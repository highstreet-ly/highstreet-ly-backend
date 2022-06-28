using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Highstreetly.Reservations.Sagas;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.SystemConsole.Themes;

namespace Highstreetly.Reservations.Processor
{
    public class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                                               .SetBasePath(Directory.GetCurrentDirectory())
                                                               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                                                               .AddJsonFile("sharedsettings.json", true, false)
                                                               .Build();
        public static void Main(string[] args)
        {
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet(settings: new JsonApiSerializer.JsonApiSerializerSettings());
            
            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                Debug.Print(msg);
            });
            
            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Information()
                         .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Error)
                         .Enrich.FromLogContext()
                         .Enrich.WithProperty("BACKEND_VERSION", Environment.GetEnvironmentVariable("BACKEND_VERSION"))
                         .Enrich.WithProperty("Application", "reservations-processor")
                         .Enrich.WithMachineName()
                         .Enrich.WithEnvironmentName()
                         .WriteTo.Console()
                         .WriteTo.PostgreSQL(
                             Configuration.GetConnectionString("TicketreservationsConnection"),
                             "logs",
                             new Dictionary<string, ColumnWriterBase>
                             {
                                 {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
                                 {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text)},
                                 {"level", new LevelColumnWriter(true, NpgsqlDbType.Varchar)},
                                 {"raise_date", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
                                 {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
                                 {"properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb)},
                                 {"props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb)},
                                 {
                                     "machine_name",
                                     new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString,
                                         NpgsqlDbType.Text, "l")
                                 }
                             }, needAutoCreateTable: false, restrictedToMinimumLevel: LogEventLevel.Information)
                         .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureAppConfiguration((builder) =>
                {
                    builder.AddConfiguration(Configuration);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (Environment.GetEnvironmentVariable("env") == "dev")
                    {
                        webBuilder.UseKestrel();
                    }
                    else
                    {
                        webBuilder.UseKestrel(options => options.Listen(IPAddress.Any, 80));
                    }

                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
    }
    
    // class Program
    // {
    //     private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
    //         .SetBasePath(Directory.GetCurrentDirectory())
    //         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    //         .AddJsonFile("sharedsettings.json", true, false)
    //         .Build();
    //
    //     static void Main(string[] args)
    //     {
    //         NpgsqlConnection.GlobalTypeMapper.UseJsonNet(settings: new JsonApiSerializer.JsonApiSerializerSettings());
    //         Console.WriteLine("Starting Reservations Processor");
    //         CreateHostBuilder(args).Build().Run();
    //     }
    //
    //     private static IHostBuilder CreateHostBuilder(string[] args) =>
    //         Host.CreateDefaultBuilder(args)
    //             .ConfigureAppConfiguration((builder) =>
    //             {
    //                 builder.AddConfiguration(Configuration);
    //             })
    //             .UseSerilog(ConfigureLogger())
    //             .ConfigureServices(ConfigureServices);
    //
    //     private static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger()
    //     {
    //         return (context, configuration) =>
    //         {
    //             Serilog.Debugging.SelfLog.Enable(msg =>
    //             {
    //                 Debug.Print(msg);
    //             });
    //
    //             configuration
    //                 .MinimumLevel.Information()
    //                 .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    //                 .MinimumLevel.Override("System", LogEventLevel.Warning)
    //                 .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
    //                 .Enrich.FromLogContext()
    //                 .Enrich.WithMachineName()
    //                 .Enrich.WithEnvironmentName()
    //                 .WriteTo.PostgreSQL(
    //                     Configuration.GetConnectionString("TicketreservationsConnection"),
    //                     "logs",
    //                     new Dictionary<string, ColumnWriterBase>
    //                     {
    //                         {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
    //                         {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text)},
    //                         {"level", new LevelColumnWriter(true, NpgsqlDbType.Varchar)},
    //                         {"raise_date", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
    //                         {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
    //                         {"properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb)},
    //                         {"props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb)},
    //                         {
    //                             "machine_name",
    //                             new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString,
    //                                 NpgsqlDbType.Text, "l")
    //                         }
    //                     }, needAutoCreateTable: false, restrictedToMinimumLevel: LogEventLevel.Information)
    //                 .Enrich.WithProperty("BACKEND_VERSION", Environment.GetEnvironmentVariable("BACKEND_VERSION"))
    //                 .Enrich.WithProperty("Application", "reservations-processor")
    //
    //                 .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: SystemConsoleTheme.Literate);
    //
    //         };
    //     }
    //
    //     private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
    //     {
    //         services.AddHsDataProtection(Configuration);
    //         services.AddOptions(Configuration);
    //         services.AddAutoMapper(typeof(MappingProfile));
    //
    //         var serializer = new JsonTextSerializer();
    //         services.AddMemoryCache();
    //         services.AddScoped<IEmailSender, EmailSender>();
    //         services.AddSingleton<ITextSerializer>(serializer);
    //         services.AddScoped<IMetadataProvider, StandardMetadataProvider>();
    //         services.AddScoped(typeof(IEventSourcedRepository<>), typeof(MartenEventStoreRepository<>));
    //         services.AddScoped<IPricingService, PricingService>();
    //         services.AddScoped<IJsonApiClientAuthProvider, JsonApiNullAuthProvider>();
    //         services.AddScoped<IEventOrganiserSiglnalrService, EventOrganiserSiglnalrService>();
    //         services.AddScoped<IUserSiglnalrService, UserSiglnalrService>();
    //
    //         services.AddJsonClient<EventInstance, Guid>();
    //         services.AddJsonClient<EventSeries, Guid>();
    //         services.AddJsonClient<EventOrganiser, Guid>();
    //         services.AddJsonClient<TicketType, Guid>();
    //         services.AddJsonClient<Payment, Guid>();
    //         services.AddJsonClient<TicketTypeConfiguration, Guid>();
    //
    //         services.AddJsonClient<Permissions.Contracts.Requests.User, Guid>();
    //         services.AddJsonClient<Permissions.Contracts.Requests.Role, Guid>();
    //         services.AddJsonClient<Permissions.Contracts.Requests.Claim, Guid>();
    //
    //         services.AddScoped<IPricingService, PricingService>();
    //         services.AddScoped((sp) => new RegistrationProcessManagerDbContextFactory().CreateDbContext(new string[] { }));
    //
    //         services.AddScoped((sp) =>
    //             new Func<IProcessManagerDataContext<RegistrationProcessManager>>(() =>
    //                 new SqlProcessManagerDataContext<RegistrationProcessManager>(
    //                     sp.GetRequiredService<RegistrationProcessManagerDbContext>,
    //                     sp.GetRequiredService<IBusClient>(),
    //                     sp.GetRequiredService<ITextSerializer>())));
    //
    //         services.AddMarten(Configuration.GetConnectionString("TicketreservationsConnection"));
    //
    //         services.AddDbContext<ReservationDbContext>(options =>
    //         {
    //             options.UseNpgsql(Configuration.GetConnectionString("TicketreservationsConnection"));
    //             options.EnableSensitiveDataLogging();
    //         });
    //
    //         var schedulerEndpoint = new Uri(Configuration["ServiceBus:Url"] + "/masstransit_quartz_scheduler");
    //
    //         services.AddMassTransit(x =>
    //         {
    //             x.AddConsumersFromNamespaceContaining<AddTicketsToBasketHandler>();
    //             x.AddConsumersFromNamespaceContaining<OrderPlacedHandler>();
    //             x.AddConsumersFromNamespaceContaining<RegistrationProcessManagerRouterIExpireRegistrationProcess>();
    //
    //             x.AddMessageScheduler(schedulerEndpoint);
    //             x.SetKebabCaseEndpointNameFormatter();
    //
    //             x.UsingRabbitMq((ctx, cfg) =>
    //             {
    //                 cfg.Host(new Uri(Configuration["ServiceBus:Url"]), hostConfigurator =>
    //                 {
    //                     hostConfigurator.Username(Configuration.GetSection("ServiceBus")["User"]);
    //                     hostConfigurator.Password(Configuration.GetSection("ServiceBus")["Password"]);
    //                 });
    //
    //                 //cfg.UseInMemoryScheduler("masstransit_quartz_scheduler");
    //
    //                 cfg.UseMessageScheduler(schedulerEndpoint);
    //                 cfg.ConfigureEndpoints(ctx);
    //             });
    //
    //         });
    //
    //         services.AddScoped<IBusClient, BusClient>();
    //
    //         services.AddMassTransitHostedService();
    //
    //         services.AddScoped<IJwtService, JwtService>();
    //         services.AddScoped<IStripeUserService, StripeUserService>();
    //         services.AddScoped<IStripeProductService, StripeProductService>();
    //         services.AddScoped<IIdentityService, IdentityService>();
    //         services.AddScoped<IStripeUserService, StripeUserService>();
    //         services.AddScoped<IStripeProductService, StripeProductService>();
    //         services.AddHttpClient();
    //       
    //     }
    // }

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
