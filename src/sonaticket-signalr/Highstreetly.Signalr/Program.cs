using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace Highstreetly.Signalr
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile($"sharedsettings.json", true, false)
                .Build();
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("BACKEND_VERSION", Environment.GetEnvironmentVariable("BACKEND_VERSION"))
                .Enrich.WithProperty("Application", "sonatribe-management-api")
                // .Enrich.WithMachineName()
                // .Enrich.WithEnvironmentName()
                .WriteTo.Console()
                // .WriteTo.PostgreSQL(
                //     configuration.GetConnectionString("TicketManagementConnection"),
                //     "logs",
                //     new Dictionary<string, ColumnWriterBase>
                //     {
                //         {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
                //         {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text)},
                //         {"level", new LevelColumnWriter(true, NpgsqlDbType.Varchar)},
                //         {"raise_date", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
                //         {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
                //         {"properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb)},
                //         {"props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb)},
                //         {
                //             "machine_name",
                //             new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString,
                //                 NpgsqlDbType.Text, "l")
                //         }
                //     }, needAutoCreateTable: false, restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();

            CreateHostBuilder(args, configuration).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfigurationRoot configuration) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builder) => { builder.AddConfiguration(configuration); })
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