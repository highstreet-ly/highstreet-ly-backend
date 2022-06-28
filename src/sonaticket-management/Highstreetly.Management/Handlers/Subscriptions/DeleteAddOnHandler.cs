using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class DeleteAddOnHandler : IConsumer<IDeleteAddOn>
    {
        private readonly ILogger<DeleteAddOnHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public DeleteAddOnHandler(ILogger<DeleteAddOnHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<IDeleteAddOn> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            var toDelete = _managementDbContext.AddOns.Where(x => x.IntegrationId == context.Message.AddOnDelete.Content.Addon.Id);

            if (toDelete.Any())
            {
                _managementDbContext.AddOns.Remove(toDelete.First());
                await _managementDbContext.SaveChangesAsync();
            }
        }
    }
}