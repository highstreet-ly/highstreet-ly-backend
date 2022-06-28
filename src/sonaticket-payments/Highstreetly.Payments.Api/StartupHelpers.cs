using System;
using System.Reflection;
using FluentValidation.AspNetCore;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Infrastructure.JsonApiClient.Auth;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.StripeIntegration;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Payments.Api.Web.ResourceDefinitions;
using Highstreetly.Payments.Api.Web.ResourceRepositories;
using Highstreetly.Payments.Resources;
using Highstreetly.Permissions.Contracts.Requests;
using Highstreetly.Reservations.Contracts.Requests;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Resources;
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

namespace Highstreetly.Payments.Api
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
            var builder = services.AddMvcCore()
                .AddFluentValidation();

            services.AddDbContext<PaymentsDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PaymentsConnection"));
                options.EnableSensitiveDataLogging();
            });

            services.AddScoped<IResourceDefinition<Refund, Guid>, RefundResourceDefinition>();
            services.AddScoped<IResourceDefinition<Payment, Guid>, PaymentResourceDefinition>();
            
            services
                .AddJsonApi<PaymentsDbContext>(
                    ConfigureJsonApiOptions, 
                    discovery => discovery.AddAssembly(Assembly.GetAssembly(typeof(RefundResourceDefinition))),
                    mvcBuilder: builder);

            services.AddSingleton<IResponseMeta, CopyrightResponseMeta>();
            // services.AddScoped<ICreateService<Payment, Guid>, PaymentService>();
            // services.AddScoped<IGetByIdService<Payment, Guid>, PaymentService>();
            // services.AddScoped<IUpdateService<Payment, Guid>, PaymentService>();
            
            services.AddJsonClient<EventInstance, Guid>();
            services.AddJsonClient<EventSeries, Guid>();
            services.AddJsonClient<EventOrganiser, Guid>();
            services.AddJsonClient<PricedOrder, Guid>();
            services.AddJsonClient<DraftOrder, Guid>();
            services.AddJsonClient<AddOn, Guid>();
            services.AddJsonClient<Plan, Guid>();
            services.AddJsonClient<User, Guid>();
            services.AddJsonClient<Role, Guid>();
            services.AddJsonClient<Claim, Guid>();
            services.AddJsonClient<TicketTypeConfiguration, Guid>();

            services.AddResourceRepository<ChargeRepository>();
            services.AddResourceRepository<LogEntryRepository>();
            services.AddResourceRepository<PaymentRepository>();
            services.AddResourceRepository<RefundRepository>();
            services.AddResourceRepository<RevenueByDayRepository>();
            services.AddResourceRepository<StatsRepository>();

            services.AddResourceDefinition<PaymentResourceDefinition>();
            services.AddResourceDefinition<RefundResourceDefinition>();

            services.AddScoped<IJsonApiClientAuthProvider, JsonApiAuthProvider>();
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
            services.AddMarten(configuration.GetConnectionString("PaymentsConnection"));

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
                    cfg.Host(new Uri(configuration["ServiceBus:Url"] ), hostConfigurator =>
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