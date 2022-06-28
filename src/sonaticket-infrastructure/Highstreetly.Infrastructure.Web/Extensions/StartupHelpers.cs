using System;
using System.IdentityModel.Tokens.Jwt;
using Highstreetly.Infrastructure.Configuration;
using Highstreetly.Infrastructure.Consul;
using Highstreetly.Infrastructure.DataProtection;
using Highstreetly.Infrastructure.Email;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.JsonApiClient.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Highstreetly.Infrastructure.Extensions
{
    public static class StartupHelpers
    {
        public static IServiceCollection AddEmailSender(this IServiceCollection services)
        {
            services.AddTransient<IEmailSender, EmailSender>();
            return services;
        }

        public static IServiceCollection AddOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var consulOptions = new ConsulConfig();
            configuration.Bind(nameof(ConsulConfig), consulOptions);
            services.AddSingleton(consulOptions);

            var emailTemplateOptions = new EmailTemplateOptions();
            configuration.Bind("EmailTemplate", emailTemplateOptions);
            services.AddSingleton(emailTemplateOptions);


            var applicationOptions = new ApplicationConfiguration();
            configuration.Bind("Application", applicationOptions);
            services.AddSingleton(applicationOptions);

            var identityServerConfiguration = new IdentityServerConfiguration();
            configuration.Bind("IdentityServer", identityServerConfiguration);
            services.AddSingleton(identityServerConfiguration);
            
            var stripeConfiguration = new StripeConfiguration();
            configuration.Bind("Stripe", stripeConfiguration);
            services.AddSingleton(stripeConfiguration);

            var twilioConfiguration = new TwilioConfiguration();
            configuration.Bind("Twilio", twilioConfiguration);
            services.AddSingleton(twilioConfiguration);

            var slackConfiguration = new SlackConfiguration();
            configuration.Bind("Slack", slackConfiguration);
            services.AddSingleton(slackConfiguration);

            return services;
        }

        
        public static IServiceCollection AddJsonClient<TModel, TId>(this IServiceCollection services)
        {
            services.AddScoped<IJsonApiClient<TModel, TId>, JsonApiClient<TModel, TId>>();
            return services;
        }

        public static IServiceCollection AddCachingJsonClient<TModel, TId>(this IServiceCollection services)
        {
            services.AddScoped<IJsonApiClient<TModel, TId>, JsonApiClient<TModel, TId>>();
            services.AddScoped<ICachingJsonApiClient<TModel, TId>, CachingJsonApiClient<TModel, TId>>();
            return services;
        }

        public static IServiceCollection AddHsDataProtection(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDataProtection().PersistKeysToPostgres(
                configuration.GetConnectionString("IdsConnection"),
                Guid.Parse("6ec8f911-bd88-4d7a-b066-768fc1d64120"),
                Guid.Parse("9e8bcecf-87ba-4171-89d2-8da7394e3fe9"));

            return services;
        }

        public static IServiceCollection AddIdentity(
            this IServiceCollection services,
            IConfiguration configuration,
            string scheme = JwtBearerDefaults.AuthenticationScheme)
        {
            Log.Information(
                $"Running AddIdentity with scheme: {JwtBearerDefaults.AuthenticationScheme}");
            
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(scheme, options =>
                {
                    options.Authority = configuration.GetIdsUrl();
                    options.Audience = configuration.GetSection("IdentityServer")["Audience"];
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            return services;
        }
    }
}