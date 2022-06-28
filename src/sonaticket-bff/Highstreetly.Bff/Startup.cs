using System;
using System.Collections.Generic;
using System.Linq;
using Highstreetly.Infrastructure.Configuration;
using Highstreetly.Infrastructure.CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;
using Ocelot.Provider.Polly;

namespace Highstreetly.Bff
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection s)
        {
            s.AddMvc(option => option.EnableEndpointRouting = false);
            
            s.AddHealthChecks();
          
            s.AddResponseCompression();
            s.AddOcelot(Configuration)
                .AddKubernetes()
                .AddCacheManager(x =>
                {
                    x.WithDictionaryHandle();
                })
                .AddPolly();

            s.AddCors(o =>
            {
                var corsOrigins = new List<string>();
                var corsOptions = new CorsConfiguration();
                Configuration.GetSection("CorsOptions").Bind(corsOptions);

                var urls = corsOptions.Urls;
                if (urls.Any())
                {
                    Console.WriteLine("adding cors with the following origins");
                    foreach (var u in urls)
                    {
                        Console.WriteLine("cors: " + u);
                    }

                    foreach (var corsOrigin in urls)
                    {
                        corsOrigins.Add(corsOrigin);
                    }
                }

                o.AddPolicy("AllowAllPolicy", options =>
                {
                    options.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders(new CorrelationIdOptions().Header, "ETag");
                });

                o.AddPolicy("SpecificCors", options =>
                {
                    options.WithOrigins(corsOrigins.ToArray())
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders(new CorrelationIdOptions().Header, "ETag");
                });
            });
            s.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var corsOptions = new CorsConfiguration();
            Configuration.GetSection("CorsOptions").Bind(corsOptions);
            if (corsOptions.UseCors)
            {
                app.UseCors("SpecificCors");
            }
            else
            {
                app.UseCors("AllowAllPolicy");
            }

            app.UseResponseCompression();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions());

                endpoints.MapHealthChecks("/", new HealthCheckOptions());
            });
            app.UseWebSockets();
            app.UseResponseCaching();
            app.UseMvc().UseOcelot().Wait();
        }
    }
}