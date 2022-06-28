using System;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Email;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Infrastructure.JsonApiClient.Auth;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.Serialization;
using Highstreetly.Infrastructure.StripeIntegration;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Management.Handlers;
using Highstreetly.Management.Handlers.Subscriptions;
using Highstreetly.Management.ReadModel;
using Highstreetly.Permissions.Contracts.Requests;
using Highstreetly.Reservations.Contracts.Requests;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Highstreetly.Management.Processor
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
            Log.Information("Configuring services");
            services.AddHsDataProtection(Configuration);
            services.AddHttpClient();
             
            services.AddOptions(Configuration);
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IStripeUserService, StripeUserService>();
            services.AddScoped<IStripeProductService, StripeProductService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IStripeUserService, StripeUserService>();
            services.AddScoped<IStripeProductService, StripeProductService>();
            services.AddScoped<ITicketQuantityService, TicketQuantityService>();
            services.AddScoped<IAzureStorage, AzureStorage>();
            services.AddHttpClient();
            var serializer = new JsonTextSerializer();
    
            services.AddMemoryCache();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddSingleton<ITextSerializer>(serializer);
            services.AddScoped<IMetadataProvider, StandardMetadataProvider>();
    
            services.AddScoped(typeof(IEventSourcedRepository<>), typeof(MartenEventStoreRepository<>));
            services.AddScoped<IEventOrganiserSiglnalrService, EventOrganiserSiglnalrService>();
            services.AddScoped<IUserSiglnalrService, UserSiglnalrService>();
    
            services.AddScoped<INotificationSenderService, NotificationSenderService>();
    
            services.AddJsonClient<User, Guid>();
            services.AddJsonClient<Role, Guid>();
            services.AddJsonClient<Claim, Guid>();
            services.AddJsonClient<DraftOrder, Guid>();
            services.AddJsonClient<PricedOrder, Guid>();
            services.AddJsonClient<User, Guid>();
             
            services.AddJsonClient<TicketTypeConfiguration, Guid>();
    
            services.AddScoped<IJsonApiClientAuthProvider, JsonApiNullAuthProvider>();
    
            services.AddMarten(Configuration.GetConnectionString("TicketManagementConnection"));
             
            services.AddDbContext<ManagementDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("TicketManagementConnection"));
                options.EnableSensitiveDataLogging();
            });
    
            services.AddAutoMapper(typeof(MappingProfile));
    
            var schedulerEndpoint = new Uri(Configuration["ServiceBus:Url"] + "/masstransit_quartz_scheduler");
    
            services.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining<TicketTypePublishedHandler>();
                x.AddConsumersFromNamespaceContaining<UnPublishEventInstanceHandlerDefinition>();
                x.AddConsumersFromNamespaceContaining<CreateAddOnHandler>();
    
                x.AddMessageScheduler(schedulerEndpoint);
                x.SetKebabCaseEndpointNameFormatter();
    
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(Configuration["ServiceBus:Url"]), hostConfigurator =>
                    {
                        hostConfigurator.Username(Configuration.GetSection("ServiceBus")["User"]);
                        hostConfigurator.Password(Configuration.GetSection("ServiceBus")["Password"]);
                    });
    
                    cfg.UseMessageScheduler(schedulerEndpoint);
                    cfg.ConfigureEndpoints(ctx);
                });
    
            });
    
            services.AddScoped<IBusClient, BusClient>();
    
            services.AddMassTransitHostedService();
    
            Log.Information("Running ");
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