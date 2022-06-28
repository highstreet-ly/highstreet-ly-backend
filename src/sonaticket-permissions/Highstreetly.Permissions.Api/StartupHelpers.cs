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
using Highstreetly.Permissions.Api.Web.ResourceDefinitions;
using Highstreetly.Permissions.Api.Web.ResourceRepositories;
using Highstreetly.Permissions.Api.Web.ResourceServices;
using Highstreetly.Permissions.Resources;
using Highstreetly.Permissions.Validators;
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
using Claim = Highstreetly.Permissions.Contracts.Requests.Claim;
using ConfirmEmailService = Highstreetly.Permissions.Api.Web.ResourceServices.ConfirmEmailService;
using Role = Highstreetly.Permissions.Contracts.Requests.Role;
using User = Highstreetly.Permissions.Contracts.Requests.User;

namespace Highstreetly.Permissions.Api
{
    public static class StartupHelpers
    {
        public static IServiceCollection AddStandardServices(
            this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IStripeUserService, StripeUserService>();
            services.AddScoped<IStripeProductService, StripeProductService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IStripeUserService, StripeUserService>();
            services.AddScoped<IStripeProductService, StripeProductService>();
            services.AddScoped<IJsonApiClientAuthProvider, JsonApiAuthProvider>();
            services.AddScoped<IAzureStorage, AzureStorage>();
            services.AddScoped<INotificationSenderService, NotificationSenderService>();
            services.AddHttpClient();
            return services;
        }

        public static IServiceCollection AddJsonApi(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var builder = services.AddMvcCore()
                .AddFluentValidation();

            services.AddTransient<IValidator<Register>, RegisterValidator>();

            services.AddDbContext<PermissionsDbContext>(
                  options =>
                  {
                      options.UseNpgsql(configuration.GetConnectionString("PermissionsConnection"));
                      options.EnableSensitiveDataLogging();
                  });

            services.AddJsonApi<PermissionsDbContext>(
                ConfigureJsonApiOptions,
                discovery => discovery.AddAssembly(Assembly.GetAssembly(typeof(UserDefinition))),
                resources: b =>
                {
                     b.Add<ForgotPassword, string>("forgot-password");
                     b.Add<Register, string>("register");
                     b.Add<ResetPassword, string>("reset-password");
                }, 
                mvcBuilder: builder);

            services.AddSingleton<IResponseMeta, CopyrightResponseMeta>();

            services.AddResourceRepository<ClaimRepository>();
            services.AddResourceRepository<LogEntryRepository>();
            services.AddResourceRepository<RoleRepository>();
            services.AddResourceRepository<UserRepository>();

            services.AddResourceDefinition<UserDefinition>();

            services.AddResourceService<RegisterService>();
            services.AddResourceService<ForgotPasswordService>();
            services.AddResourceService<ResetPasswordService>();
            services.AddResourceService<ConfirmEmailService>();

            services.AddScoped<IJsonApiClientAuthProvider, JsonApiAuthProvider>();
            services.AddJsonClient<EventOrganiser, Guid>();
            services.AddJsonClient<User, Guid>();
            services.AddJsonClient<Role, Guid>();
            services.AddJsonClient<Claim, Guid>();
            services.AddJsonClient<EventInstance, Guid>();
            services.AddJsonClient<TicketTypeConfiguration, Guid>();

            return services;
        }

        private static void ConfigureJsonApiOptions(
            JsonApiOptions options)
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
            services.AddMarten(configuration.GetConnectionString("PermissionsConnection"));

            return services;
        }

        public static IServiceCollection AddMassTransit(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();

            LogContext.ConfigureCurrentLogContext(serviceProvider.GetService<ILoggerFactory>());
            var schedulerEndpoint = new Uri(configuration["ServiceBus:Url"] + "/masstransit_quartz_scheduler");

            // var auditConnectionString =   configuration.GetConnectionString("AuditConnection");
            // var auditDocStore = DocumentStore.For(_ =>
            // {
            //     _.Connection(auditConnectionString);
            // });

            //var auditStore = new MartenAuditStore();

            services.AddMassTransit(
                x =>
                {
                    x.AddMessageScheduler(schedulerEndpoint);
                    x.SetKebabCaseEndpointNameFormatter();

                    x.UsingRabbitMq(
                        (
                            ctx,
                            cfg) =>
                        {
                            cfg.Host(
                                new Uri(configuration["ServiceBus:Url"]),
                                hostConfigurator =>
                                {
                                    hostConfigurator.Username(configuration.GetSection("ServiceBus")["User"]);
                                    hostConfigurator.Password(configuration.GetSection("ServiceBus")["Password"]);
                                });

                            // // cfg.ConnectConsumeAuditObserver(auditStore);
                            // // cfg.ConnectSendAuditObservers(auditStore);
                            //
                            cfg.ConfigureEndpoints(ctx);
                        });
                });

            services.AddScoped<IBusClient, BusClient>();

            return services;
        }
    }
}