using System;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.DataBase;
using Highstreetly.Infrastructure.Email;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Infrastructure.JsonApiClient.Auth;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.Serialization;
using Highstreetly.Infrastructure.StripeIntegration;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Payments.Domain;
using Highstreetly.Payments.ReadModel;
using Highstreetly.Reservations.Contracts.Requests;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using CancelThirdPartyProcessorPaymentHandler = Highstreetly.Payments.Handlers.CancelThirdPartyProcessorPaymentHandler;

namespace Highstreetly.Payments.Processor
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
            services.AddMemoryCache();
            services.AddScoped<IEmailSender, EmailSender>();
    
            var schedulerEndpoint = new Uri(Configuration["ServiceBus:Url"] + "/masstransit_quartz_scheduler");
    
            services.AddDbContext<PaymentsDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("PaymentsConnection"));
                options.EnableSensitiveDataLogging();
            });
    
            services.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining<CancelThirdPartyProcessorPaymentHandler>();
                x.AddConsumersFromNamespaceContaining<ApplicationFeeCreatedHandler>();
    
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
    
            var serializer = new JsonTextSerializer();
    
            services.AddSingleton<ITextSerializer>(serializer);
            services.AddScoped<IMetadataProvider, StandardMetadataProvider>();
            services.AddScoped(typeof(IEventSourcedRepository<>), typeof(MartenEventStoreRepository<>));
            services.AddScoped<IEventOrganiserSiglnalrService, EventOrganiserSiglnalrService>();
            services.AddScoped<IUserSiglnalrService, UserSiglnalrService>();
    
            services.AddMarten(Configuration.GetConnectionString("PaymentsConnection"));
    
            services.AddScoped((sp) =>
                new Func<IDataContext<ThirdPartyProcessorPayment>>(() =>
                    new SqlDataContext<ThirdPartyProcessorPayment>(
                        sp.GetRequiredService<PaymentsDbContext>,
                        sp.GetRequiredService<IBusControl>()))
            );
    
            services.AddScoped((sp) => new PaymentsDbContextFactory().CreateDbContext(new string[] { }));
            services.AddAutoMapper(typeof(MappingProfile));
    
            services.AddJsonClient<Permissions.Contracts.Requests.User, Guid>();
            services.AddJsonClient<Permissions.Contracts.Requests.Role, Guid>();
            services.AddJsonClient<Permissions.Contracts.Requests.Claim, Guid>();
            services.AddJsonClient<Order, Guid>();
            services.AddJsonClient<EventInstance, Guid>();
            services.AddJsonClient<EventOrganiser, Guid>();
            services.AddJsonClient<Plan, Guid>();
            services.AddJsonClient<TicketTypeConfiguration, Guid>();
            services.AddJsonClient<PricedOrder, Guid>();
    
            services.AddScoped<IJsonApiClientAuthProvider, JsonApiNullAuthProvider>();
    
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IStripeUserService, StripeUserService>();
            services.AddScoped<IStripeProductService, StripeProductService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IStripeUserService, StripeUserService>();
            services.AddScoped<IStripeProductService, StripeProductService>();
            services.AddHttpClient();
            services.AddMassTransitHostedService();
    
            services.AddScoped<IAzureStorage, AzureStorage>();
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