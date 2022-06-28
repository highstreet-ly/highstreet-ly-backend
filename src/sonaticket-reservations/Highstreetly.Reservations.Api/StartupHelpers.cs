using System;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Infrastructure.JsonApiClient.Auth;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.StripeIntegration;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Permissions.Contracts.Requests;
using Highstreetly.Reservations.Api.Web.ResourceDefinitions;
using Highstreetly.Reservations.Api.Web.ResourceRepositories;
using Highstreetly.Reservations.Distance;
using Highstreetly.Reservations.Resources;
using Highstreetly.Reservations.Validators;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Serialization;
using Marten;
using MassTransit;
using MassTransit.Context;
using MassTransit.Definition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Highstreetly.Reservations.Api
{
    public static class StartupHelpers
    {
        public static IServiceCollection AddStandardServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IStripeUserService, StripeUserService>();
            services.AddScoped<IStripeProductService, StripeProductService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IStripeUserService, StripeUserService>();
            services.AddScoped<IStripeProductService, StripeProductService>();
            services.AddScoped<IJsonApiClientAuthProvider, JsonApiAuthProvider>();
            services.AddScoped<IAzureStorage, AzureStorage>();
            services.AddHttpClient();
            return services;
        }
        public static IServiceCollection AddJsonApi(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var builder = services.AddMvcCore();

            // NOTE:  we cannot do this here because it seems to break /operations
                //.AddFluentValidation();

            services.AddTransient<IValidator<DraftOrder>, DraftOrderValidator>();

            services.AddDbContext<ReservationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("TicketreservationsConnection")));
            services
                .AddJsonApi<ReservationDbContext>(
                    ConfigureJsonApiOptions,
                    discovery => discovery.AddAssembly(Assembly.GetAssembly(typeof(DraftOrderResourceDefinition))),
                    mvcBuilder: builder);

            services.AddSingleton<IResponseMeta, CopyrightResponseMeta>();

            services.AddCachingJsonClient<EventInstance, Guid>();
            services.AddCachingJsonClient<EventOrganiser, Guid>();
            services.AddCachingJsonClient<EventSeries, Guid>();
            services.AddCachingJsonClient<TicketType, Guid>();
            services.AddCachingJsonClient<User, Guid>();
            services.AddCachingJsonClient<Role, Guid>();
            services.AddCachingJsonClient<Claim, Guid>();
            services.AddCachingJsonClient<TicketTypeConfiguration, Guid>();

            services.AddScoped<IDistanceApi, DistanceApi>();
            services.AddScoped<IJsonApiClientAuthProvider, JsonApiAuthProvider>();

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddResourceDefinition<DraftOrderResourceDefinition>();

            services.AddResourceRepository<DraftOrderItemRepository>();
            services.AddResourceRepository<DraftOrderRepository>();
            services.AddResourceRepository<LogEntryRepository>();
            services.AddResourceRepository<OrderTicketDetailsRepository>();
            services.AddResourceRepository<PricedOrderRepository>();
            // services.AddResourceRepository<ProductExtraRepository>();
            
            return services;
        }

        private static void ConfigureJsonApiOptions(JsonApiOptions options)
        {
            options.Namespace = "api/v1";
            options.UseRelativeLinks = true;
            options.IncludeTotalResourceCount = true;
            options.AllowClientGeneratedIds = true;
            options.AllowUnknownQueryStringParameters = true;
            options.EnableLegacyFilterNotation = true;
            options.DefaultPageSize = new PageSize(100);

            options.ValidateModelState = true;

            options.SerializerSettings.Formatting = Formatting.Indented;
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.MaximumOperationsPerRequest = null;
            options.SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new KebabCaseNamingStrategy()
            };
        }

        public static IServiceCollection AddMartenDb(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMarten(configuration.GetConnectionString("TicketreservationsConnection"));
            return services;
        }

        public static void CacheCert(this IServiceCollection services)
        {
            var sb = services.BuildServiceProvider();
            var cache = sb.GetService<IMemoryCache>();
            var jwtService = sb.GetService<IJwtService>();

            var cacheEntryOptions = new MemoryCacheEntryOptions();
            cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromSeconds(6000));
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(6000);
            var certificate = jwtService.LoadCertificate();
            cache.Set(
                "jwtCertificate",
                certificate,
                cacheEntryOptions);
        }

        public static IServiceCollection AddMassTransit(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();

            LogContext.ConfigureCurrentLogContext(serviceProvider.GetService<ILoggerFactory>());
            var schedulerEndpoint = new Uri(configuration["ServiceBus:Url"] + "/masstransit_quartz_scheduler");
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

            services.AddMassTransit(x =>
            {
                x.AddMessageScheduler(schedulerEndpoint);
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(configuration["ServiceBus:Url"]), hostConfigurator =>
                   {
                       hostConfigurator.Username(configuration.GetSection("ServiceBus")["User"]);
                       hostConfigurator.Password(configuration.GetSection("ServiceBus")["Password"]);
                   });

                    cfg.ConfigureEndpoints(ctx);
                });

            });

            services.AddScoped<IBusClient, BusClient>();

            return services;
        }
    }
}