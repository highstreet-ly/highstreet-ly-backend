using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.CorrelationId;
using Highstreetly.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using JsonApiDotNetCore.Configuration;
using Microsoft.Extensions.Configuration;

namespace Highstreetly.Payments.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
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

            services.AddScoped<IAzureStorage, AzureStorage>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
