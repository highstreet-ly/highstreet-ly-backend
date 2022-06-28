using System;
using Highstreetly.Ids.Services;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.DataProtection;
using Highstreetly.Infrastructure.Email;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.JsonApiClient.Auth;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Permissions.Resources;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Highstreetly.Ids
{
    public static class StartupHelpers
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Permissions.PermissionsDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("IdsConnection"));
                options.EnableSensitiveDataLogging();
            });

            return services;
        }

        public static IServiceCollection AddIdentityServer(this IServiceCollection services,
            IConfiguration configuration)
        {
            string issuerUrl;
            var connectionString = configuration.GetConnectionString("IdsConnection");

            if (Environment.GetEnvironmentVariable("env") == "dev")
            {
                issuerUrl = "https://ids.highstreetly.xyz";
            }
            else
            {
                const string scheme = "https://";
                issuerUrl = $"{scheme}ids.{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}";
            }

            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<Permissions.PermissionsDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IProfileService, ProfileService>();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddJsonClient<Subscription, Guid>();
            services.AddJsonClient<User, Guid>();


            services.AddScoped<IJsonApiClientAuthProvider, JsonApiAuthProvider>();

            var idsBuilder = services.AddIdentityServer(options =>
                {
                    options.IssuerUri = issuerUrl;
                })
                .AddAspNetIdentity<User>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(connectionString);
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(connectionString);
                })
                .AddProfileService<ProfileService>()
                .AddCustomTokenRequestValidator<PoopRequestValidator>()
                .AddInMemoryCaching()
                .AddConfigurationStoreCache();


            var jwt = new JwtService(configuration, null);
            idsBuilder.AddSigningCredential(jwt.LoadCertificate());

            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = issuerUrl;
                });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.Cookie.Domain = $".{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}";
                options.SlidingExpiration = true;
            });

            return services;
        }
    }
}