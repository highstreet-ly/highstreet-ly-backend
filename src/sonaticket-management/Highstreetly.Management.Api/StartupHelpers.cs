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
using Highstreetly.Management.Api.Services;
using Highstreetly.Management.Api.Web.ResourceDefinitions;
using Highstreetly.Management.Api.Web.ResourceRepositories;
using Highstreetly.Management.Resources;
using Highstreetly.Management.Validators;
using Highstreetly.Permissions.Contracts.Requests;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Serialization;
using JsonApiDotNetCore.Services;
using Marten;
using MassTransit;
using MassTransit.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using EventOrganiser = Highstreetly.Management.Resources.EventOrganiser;
using EventSeries = Highstreetly.Management.Resources.EventSeries;
using Role = Highstreetly.Permissions.Contracts.Requests.Role;

namespace Highstreetly.Management.Api
{
    public static class StartupHelpers
    {

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
            var builder = services.AddMvcCore()
                .AddFluentValidation();

            services.AddTransient<IValidator<EventInstance>, EventInstanceValidator>();
            services.AddTransient<IValidator<EventOrganiser>, EventOrganiserValidator>();
            services.AddTransient<IValidator<TicketTypeConfiguration>, TicketTypeConfigurationValidator>();

            services.AddDbContext<ManagementDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("TicketManagementConnection"));
                options.EnableSensitiveDataLogging();
            });

            services
                .AddJsonApi<ManagementDbContext>(
                    ConfigureJsonApiOptions, 
                    discovery => discovery.AddAssembly(Assembly.GetAssembly(typeof(ProductExtraGroupDefinition))),
                    mvcBuilder: builder);

            services.AddSingleton<IResponseMeta, CopyrightResponseMeta>();

            services.AddScoped<IResourceService<EventOrganiser, Guid>, EventOrganiserService>();
            services.AddScoped<IResourceService<EventSeries, Guid>, EventSeriesService>();
            services.AddScoped<IResourceService<EventInstance, Guid>, EventInstanceService>();
            services.AddScoped<IResourceService<TicketTypeConfiguration, Guid>, TicketTypeConfigurationService>();
            services.AddScoped<IResourceService<Order, Guid>, OrderService>();

            services.AddJsonClient<User, Guid>();
            services.AddJsonClient<Role, Guid>();
            services.AddJsonClient<Claim, Guid>();
            services.AddJsonClient<Highstreetly.Management.Contracts.Requests.TicketTypeConfiguration, Guid>();

            services.AddResourceRepository<AddOnRepository>();
            services.AddResourceRepository<BusinessTypeFeatureTemplateRepository>();
            services.AddResourceRepository<BusinessTypeRepository>();
            services.AddResourceRepository<EventInstanceRepository>();
            services.AddResourceRepository<EventOrganiserRepository>();
            services.AddResourceRepository<EventSeriesRepository>();
            services.AddResourceRepository<FeatureRepository>();
            services.AddResourceRepository<ImageRepository>();
            services.AddResourceRepository<LogEntryRepository>();
            services.AddResourceRepository<OrderRepository>();
            services.AddResourceRepository<PlanRepository>();
            services.AddResourceRepository<ProductCategoryRepository>();
            services.AddResourceRepository<ProductExtraGroupRepository>();
            services.AddResourceRepository<ProductExtraRepository>();
            services.AddResourceRepository<SubscriptionRepository>();
            services.AddResourceRepository<TicketTypeConfigurationRepository>();
            services.AddResourceRepository<TicketTypeRepository>();

            services.AddResourceDefinition<EventInstanceDefinition>();
            services.AddResourceDefinition<EventOrganiserDefinition>();
            services.AddResourceDefinition<ProductExtraDefinition>();
            services.AddResourceDefinition<PlanDefinition>();
            services.AddResourceDefinition<ProductExtraGroupDefinition>();

            services.AddAutoMapper(typeof(MappingProfile));
            return services;
        }

        public static IServiceCollection AddMartenDb(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMarten(configuration.GetConnectionString("TicketManagementConnection"));
            return services;
        }

        public static IServiceCollection AddMassTransit(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();

            LogContext.ConfigureCurrentLogContext(serviceProvider.GetService<ILoggerFactory>());
            var schedulerEndpoint = new Uri(configuration["ServiceBus:Url"] + "/masstransit_quartz_scheduler");

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