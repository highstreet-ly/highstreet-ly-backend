using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Highstreetly.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace Highstreetly.Ids
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
            Console.WriteLine("Starting Ids");
            // todo: this should be using certs validation in flogger.cs
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
           
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("BACKEND_VERSION", Environment.GetEnvironmentVariable("BACKEND_VERSION"))
                .Enrich.WithProperty("Application", "ids")
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console()
                .WriteTo.PostgreSQL(
                    Configuration.GetConnectionString("IdsConnection"),
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

            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                Debug.Print(msg);
            });

            var host = CreateHostBuilder(args).Build();

            try
            {
                Console.WriteLine("Starting Ids: host.Run");
                SeedData.EnsureSeedData(host.Services);
                host.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
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
                    Console.WriteLine("Starting Ids: ConfigureWebHostDefaults");
                    if (Environment.GetEnvironmentVariable("env") == "dev")
                    {
                        Console.WriteLine("Starting Ids: ConfigureWebHostDefaults: dev config");
                        webBuilder.UseKestrel();
                    }
                    else
                    {
                        Console.WriteLine("Starting Ids: ConfigureWebHostDefaults: live config");
                       
                        webBuilder.ConfigureKestrel(k =>
                        {
                            k.Listen(IPAddress.Any, 80);
                            var jwt = new JwtService(Log.Logger, null);
                            k.ConfigureHttpsDefaults(options =>
                            {
                                Console.WriteLine("Starting Ids: ConfigureWebHostDefaults: load certs");
                                options.ServerCertificate = jwt.LoadCertificate();
                            });
                        });
                    }

                    webBuilder.UseStartup<Startup>();
                }).UseSerilog();
    }
}