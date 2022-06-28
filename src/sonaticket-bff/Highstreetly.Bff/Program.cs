using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
// using Microsoft.IdentityModel.Logging;

namespace Highstreetly.Bff
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            // todo: this should be using certs validation in flogger.cs
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            //IdentityModelEventSource.ShowPII = true;
            BuildWebHost(args).Run();
        }

        public static IHost BuildWebHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, false)
                        .AddJsonFile($"ocelot.json", true, false)
                        .AddJsonFile($"sharedsettings.json", true, false);

                    config.AddEnvironmentVariables();

                    Configuration = config.Build();
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
                .Build();
    }
}