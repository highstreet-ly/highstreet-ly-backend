using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class PauseUserSubscriptionHandler : IConsumer<IPauseUserSubscription>
    {
        private readonly ILogger<PauseUserSubscriptionHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public PauseUserSubscriptionHandler(ILogger<PauseUserSubscriptionHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<IPauseUserSubscription> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            var incomingSubscription = context.Message.SubscriptionPause.Content.Subscription;

            var subscription = _managementDbContext.Subscriptions.Single(x =>
                x.IntegrationId == incomingSubscription.Id);

            subscription.PauseDate = incomingSubscription.PauseDate;

            await _managementDbContext.SaveChangesAsync();

        }
    }
}