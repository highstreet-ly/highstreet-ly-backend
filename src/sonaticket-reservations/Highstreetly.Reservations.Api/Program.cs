using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Npgsql;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace Highstreetly.Reservations.Api
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
            IdentityModelEventSource.ShowPII = true;
            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                Debug.Print(msg);
            });

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("BACKEND_VERSION", Environment.GetEnvironmentVariable("BACKEND_VERSION"))
                .Enrich.WithProperty("Application", "reservations-api")
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

            NpgsqlConnection.GlobalTypeMapper.UseJsonNet();

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
}
