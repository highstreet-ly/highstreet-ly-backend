using System;
using System.Collections.Generic;
using System.Linq;
using Highstreetly.Infrastructure.CorrelationId;
using MassTransit;
using MassTransit.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Highstreetly.Infrastructure.Extensions;

namespace Highstreetly.Signalr
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        
        public static IConfiguration Configuration { get; set; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddControllers();
            services.AddOptions(Configuration);

            services.AddMassTransit(x =>
            {
                x.AddSignalRHub<ChatHub>();

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri(Configuration["ServiceBus:Url"]), h =>
                    {
                        h.Username(Configuration.GetSection("ServiceBus")["User"]);
                        h.Password(Configuration.GetSection("ServiceBus")["Password"]);
                    });

                    cfg.ConfigureEndpoints(provider);
                }));
            });
            
            services.AddCors(o =>
            {
                var corsOrigins = new List<string>();
                var corsOptions = new CorsOptions();
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

            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var corsOptions = new CorsOptions();
            Configuration.GetSection("CorsOptions").Bind(corsOptions);
            if (corsOptions.UseCors)
            {
                app.UseCors("SpecificCors");
            }
            else
            {
                app.UseCors("AllowAllPolicy");
            }
            
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/connection");
                endpoints.MapControllers();
            });
        }
    }
}