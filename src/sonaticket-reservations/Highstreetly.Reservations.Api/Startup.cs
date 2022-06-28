using System;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.CorrelationId;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.Processors;
using Highstreetly.Infrastructure.Serialization;
using Highstreetly.Reservations.Sagas;
using JsonApiDotNetCore.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Highstreetly.Reservations.Api
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
            services.AddHsDataProtection(Configuration);
            services.AddResponseCompression();
            services.AddStandardServices();
            services.AddOptions(Configuration);
            services.AddControllers();
            services.AddJsonApi(Configuration);
            services.AddMartenDb(Configuration);
            services.AddMassTransit(Configuration);
            services.AddIdentity(Configuration);
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddEmailSender();

            // find a new home
            var serializer = new JsonTextSerializer();
            services.AddSingleton<ITextSerializer>(serializer);
            services.AddScoped<IMetadataProvider, StandardMetadataProvider>();
            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped(typeof(IEventSourcedRepository<>), typeof(MartenEventStoreRepository<>));
            services.AddScoped((sp) => new RegistrationProcessManagerDbContextFactory().CreateDbContext(Array.Empty<string>()));

            services.AddScoped((sp) =>
                new Func<IProcessManagerDataContext<RegistrationProcessManager>>(() =>
                    new SqlProcessManagerDataContext<RegistrationProcessManager>(
                        sp.GetRequiredService<RegistrationProcessManagerDbContext>,
                        sp.GetRequiredService<IBusClient>(),
                        sp.GetRequiredService<ITextSerializer>())));

            services.CacheCert();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders();
            app.UseCorrelationId(new CorrelationIdOptions());
            app.UseResponseCompression();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseJsonApi();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
