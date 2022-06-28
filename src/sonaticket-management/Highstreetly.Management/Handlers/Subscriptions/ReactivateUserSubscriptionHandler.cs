using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class ReactivateUserSubscriptionHandler : IConsumer<IReactivateUserSubscription>
    {
        private readonly ILogger<ReactivateUserSubscriptionHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public ReactivateUserSubscriptionHandler(ILogger<ReactivateUserSubscriptionHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<IReactivateUserSubscription> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            var subscriptions = _managementDbContext.Subscriptions.Where(x =>
                x.IntegrationId == context.Message.SubscriptionReactivate.Content.Subscription.Id);

            foreach (var subscription in subscriptions)
            {
                subscription.ActivatedAt = context.Message.SubscriptionReactivate.Content.Subscription.ActivatedAt;
                subscription.Status = context.Message.SubscriptionReactivate.Content.Subscription.Status;
            }

            await _managementDbContext.SaveChangesAsync();
        }
    }
}