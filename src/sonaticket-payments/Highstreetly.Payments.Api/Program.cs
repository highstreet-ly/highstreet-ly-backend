using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace Highstreetly.Payments.Api
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
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Error)
                .Enrich.WithProperty("BACKEND_VERSION", Environment.GetEnvironmentVariable("BACKEND_VERSION"))
                .Enrich.WithProperty("Application", "payments-api")
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console()
                .WriteTo.PostgreSQL(
                    Configuration.GetConnectionString("PaymentsConnection"),
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
}
