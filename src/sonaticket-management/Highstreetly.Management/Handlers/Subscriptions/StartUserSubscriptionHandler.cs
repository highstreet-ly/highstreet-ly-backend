using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class StartUserSubscriptionHandler : IConsumer<IStartUserSubscription>
    {
        private readonly ILogger<StartUserSubscriptionHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public StartUserSubscriptionHandler(ILogger<StartUserSubscriptionHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<IStartUserSubscription> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");
            var incomingSubscription = context.Message.SubscriptionStart.Content.Subscription;

            var subscription = _managementDbContext.Subscriptions.Single(x =>
                x.IntegrationId == incomingSubscription.Id);

            subscription.StartedAt = incomingSubscription.StartedAt;

            await _managementDbContext.SaveChangesAsync();
        }
    }
}