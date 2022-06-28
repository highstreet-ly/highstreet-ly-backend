using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using GreenPipes;
using MassTransit;
using MassTransit.QuartzIntegration;
using MassTransit.Scheduling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NpgsqlTypes;
using Quartz;
using Quartz.Impl;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.SystemConsole.Themes;

namespace Highstreetly.Scheduler
{
    class Program
    {
        static readonly string _queueName = "masstransit_quartz_scheduler";
        
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("sharedsettings.json", true, false)
            .Build();
        
        public static void Main(string[] args)
        {
            // todo: this should be using certs validation in flogger.cs
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("BACKEND_VERSION", Environment.GetEnvironmentVariable("BACKEND_VERSION"))
                .Enrich.WithProperty("Application", "sonatribe-scheduler")
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    theme: SystemConsoleTheme.Literate)
                .WriteTo.PostgreSQL(
                    Configuration.GetConnectionString("SchedulerConnection"),
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
            Log.Information("Starting Scheduler - highstreet.ly");
            
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builder) =>
                {
                    builder.AddConfiguration(Configuration);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(mt =>
                    {
                        mt.UsingRabbitMq((context, cfg) =>
                        {
                            var scheduler = context.GetRequiredService<IScheduler>();
                            
                            Log.Information($"Starting rmq with url {Configuration["ServiceBus:Url"]}");

                            cfg.Host(Configuration["ServiceBus:Url"], h =>
                            {
                                h.Username(hostContext.Configuration.GetSection("ServiceBus")["User"]);
                                h.Password(hostContext.Configuration.GetSection("ServiceBus")["Password"]);
                            });

                            cfg.UseJsonSerializer(); // Because we are using json within Quartz for serializer type

                            cfg.ReceiveEndpoint(_queueName, endpoint =>
                            {
                                var partitionCount = Environment.ProcessorCount;
                                endpoint.PrefetchCount = (ushort)(partitionCount);
                                var partitioner = endpoint.CreatePartitioner(partitionCount);

                                endpoint.Consumer(() => new ScheduleMessageConsumer(scheduler), x =>
                                    x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId)));
                                endpoint.Consumer(() => new CancelScheduledMessageConsumer(scheduler),
                                    x => x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId)));
                            });
                            
                        });
                    });

                    services.AddHostedService<MassTransitConsoleHostedService>();

                    services.AddSingleton(x =>
                    {
                        var quartzConfig = new QuartzConfig()
                            .ToNameValueCollection(hostContext.Configuration);
                        return new StdSchedulerFactory(quartzConfig).GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();

                    });
                });

    }
}