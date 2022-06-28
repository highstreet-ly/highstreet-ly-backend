using System;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Email;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Infrastructure.JsonApiClient.Auth;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.Processors;
using Highstreetly.Infrastructure.Serialization;
using Highstreetly.Infrastructure.StripeIntegration;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Payments.Contracts.Requests;
using Highstreetly.Reservations.Handlers;
using Highstreetly.Reservations.ReadModel;
using Highstreetly.Reservations.Sagas;
using Marten;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Highstreetly.Reservations.Processor
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
            services.AddAutoMapper(typeof(MappingProfile));

            var serializer = new JsonTextSerializer();
            services.AddMemoryCache();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddSingleton<ITextSerializer>(serializer);
            services.AddScoped<IMetadataProvider, StandardMetadataProvider>();
            services.AddScoped(typeof(IEventSourcedRepository<>), typeof(MartenEventStoreRepository<>));
            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped<IJsonApiClientAuthProvider, JsonApiNullAuthProvider>();
            services.AddScoped<IEventOrganiserSiglnalrService, EventOrganiserSiglnalrService>();
            services.AddScoped<IUserSiglnalrService, UserSiglnalrService>();

            services.AddCachingJsonClient<EventInstance, Guid>();
            services.AddCachingJsonClient<EventSeries, Guid>();
            services.AddCachingJsonClient<EventOrganiser, Guid>();
            services.AddCachingJsonClient<TicketType, Guid>();
            services.AddJsonClient<Payment, Guid>();
            services.AddCachingJsonClient<TicketTypeConfiguration, Guid>();

            services.AddJsonClient<Permissions.Contracts.Requests.User, Guid>();
            services.AddJsonClient<Permissions.Contracts.Requests.Role, Guid>();
            services.AddJsonClient<Permissions.Contracts.Requests.Claim, Guid>();

            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped((sp) =>
                new RegistrationProcessManagerDbContextFactory().CreateDbContext(new string[] { }));

            services.AddScoped((sp) =>
                new Func<IProcessManagerDataContext<RegistrationProcessManager>>(() =>
                    new SqlProcessManagerDataContext<RegistrationProcessManager>(
                        sp.GetRequiredService<RegistrationProcessManagerDbContext>,
                        sp.GetRequiredService<IBusClient>(),
                        sp.GetRequiredService<ITextSerializer>())));

            services.AddMarten(Configuration.GetConnectionString("TicketreservationsConnection"));

            services.AddDbContext<ReservationDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("TicketreservationsConnection"));
                options.EnableSensitiveDataLogging();
            });

            var schedulerEndpoint = new Uri(Configuration["ServiceBus:Url"] + "/masstransit_quartz_scheduler");

            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

            services.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining<AddTicketsToBasketHandler>();
                x.AddConsumersFromNamespaceContaining<OrderPlacedHandler>();
                x.AddConsumersFromNamespaceContaining<RegistrationProcessManagerRouterIExpireRegistrationProcess>();

                x.AddMessageScheduler(schedulerEndpoint);
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(Configuration["ServiceBus:Url"]), hostConfigurator =>
                    {
                        hostConfigurator.Username(Configuration.GetSection("ServiceBus")["User"]);
                        hostConfigurator.Password(Configuration.GetSection("ServiceBus")["Password"]);
                    });

                    //cfg.UseInMemoryScheduler("masstransit_quartz_scheduler");

                    cfg.UseMessageScheduler(schedulerEndpoint);
                    cfg.ConfigureEndpoints(ctx);
                });

            });

            services.AddScoped<IBusClient, BusClient>();

            services.AddMassTransitHostedService();

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