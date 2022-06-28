using System;
using System.Collections.Generic;
using System.Linq;
using Highstreetly.Infrastructure.CorrelationId;
using Highstreetly.Infrastructure.Extensions;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Highstreetly.Ids
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddHsDataProtection(Configuration);
            services.AddOptions(Configuration);
            services.AddControllers();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddEmailSender();
            services.AddDbContext(Configuration);
            services.AddAuthorization();
            services.AddIdentityServer(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders();
            app.UseCorrelationId(new CorrelationIdOptions());
            
            app.Use(async (ctx, next) =>
            {
                ctx.SetIdentityServerOrigin($"https://ids.{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}");
                await next();
            });
            
            app.UseCors(builder =>
            {
                var corsOptions = new CorsOptions();
                Configuration.GetSection("CorsOptions").Bind(corsOptions);
            
                if (corsOptions.UseCors)
                {
                    var urls = corsOptions.Urls;
            
                    if (urls.Any())
                    {
                        Console.WriteLine("adding cors with the following origins");
                        foreach (var u in urls)
                        {
                            Console.WriteLine("cors: " + u);
                        }
            
                        var corsOrigins = new List<string>();
                        foreach (var corsOrigin in urls)
                        {
                            corsOrigins.Add(corsOrigin);
                        }
            
                        var origins = new string[corsOrigins.Count];
                        corsOrigins.CopyTo(origins, 0);
            
                        builder.WithOrigins(origins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                    else
                    {
                        Console.WriteLine("adding cors with any origin");
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                }
            });

            app.UseRouting();
            
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions());
                
                endpoints.MapHealthChecks("/", new HealthCheckOptions());
            });
        }
    }
}