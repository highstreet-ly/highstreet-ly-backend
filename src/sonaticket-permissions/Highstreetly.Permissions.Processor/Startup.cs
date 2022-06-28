using System;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Email;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Infrastructure.JsonApiClient.Auth;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.StripeIntegration;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Permissions.Handlers;
using Highstreetly.Permissions.Handlers.Subscriptions;
using Highstreetly.Permissions.Resources;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Highstreetly.Permissions.Processor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = (check) => check.Tags.Contains("ready");
            });
            
            services.AddHsDataProtection(Configuration);
            services.AddOptions(Configuration);
            services.AddScoped<IJsonApiClientAuthProvider, JsonApiNullAuthProvider>();

            services.AddMarten(Configuration.GetConnectionString("PermissionsConnection"));

            services.AddJsonClient<EventInstance, Guid>();
            services.AddJsonClient<EventSeries, Guid>();
            services.AddJsonClient<EventOrganiser, Guid>();
            services.AddJsonClient<Contracts.Requests.User, Guid>();
            services.AddJsonClient<Contracts.Requests.Role, Guid>();
            services.AddJsonClient<Contracts.Requests.Claim, Guid>();
            services.AddJsonClient<TicketTypeConfiguration, Guid>();
            services.AddScoped<INotificationSenderService, NotificationSenderService>();
            services.AddMemoryCache();
            services.AddScoped<IEmailSender, EmailSender>();

            var schedulerEndpoint = new Uri(Configuration["ServiceBus:Url"] + "/masstransit_quartz_scheduler");

            services.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining<RegisterB2BUserHandler>();
                x.AddConsumersFromNamespaceContaining<CancelUserSubscriptionHandler>();

                x.AddMessageScheduler(schedulerEndpoint);
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(Configuration["ServiceBus:Url"]), hostConfigurator =>
                    {
                        hostConfigurator.Username(Configuration.GetSection("ServiceBus")["User"]);
                        hostConfigurator.Password(Configuration.GetSection("ServiceBus")["Password"]);
                    });

                    // cfg.ConnectConsumeAuditObserver(auditStore);
                    // cfg.ConnectSendAuditObservers(auditStore);

                    cfg.UseMessageScheduler(schedulerEndpoint);
                    cfg.ConfigureEndpoints(ctx);
                });

            });

            services.AddMassTransitHostedService();

            services.AddScoped<IBusClient, BusClient>();

            services.AddDbContext<PermissionsDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("PermissionsConnection"));
                options.EnableSensitiveDataLogging();
            });

            services.AddIdentity<User, Role>(options =>
                    {
                        // Basic built in validations
                        options.Password.RequireDigit = true;
                        options.Password.RequireLowercase = true;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = true;
                        options.Password.RequiredLength = 6;
                    })
                    .AddEntityFrameworkStores<PermissionsDbContext>()
                    .AddDefaultTokenProviders();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IStripeUserService, StripeUserService>();
            services.AddScoped<IStripeProductService, StripeProductService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IStripeUserService, StripeUserService>();
            services.AddScoped<IStripeProductService, StripeProductService>();
            services.AddHttpClient();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions()
                                                           {
                                                               Predicate = (check) => check.Tags.Contains("ready"),
                                                           });

                endpoints.MapHealthChecks("/", new HealthCheckOptions());
            });
        }
    }
}