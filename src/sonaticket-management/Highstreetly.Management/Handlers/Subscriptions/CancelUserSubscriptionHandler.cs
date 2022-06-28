using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class CancelUserSubscriptionHandler : IConsumer<ICancelUserSubscription>
    {
        private readonly ILogger<CancelUserSubscriptionHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public CancelUserSubscriptionHandler(ILogger<CancelUserSubscriptionHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<ICancelUserSubscription> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            var incomingSubscription = context.Message.SubscriptionCancel.Content.Subscription;

            var subscriptions = _managementDbContext.Subscriptions.Where(x =>
                x.IntegrationId == incomingSubscription.Id);

            foreach (var subscription in subscriptions)
            {
                subscription.CancelledAt = incomingSubscription.CancelledAt;
                subscription.Status = incomingSubscription.Status;
            }

            await _managementDbContext.SaveChangesAsync();
        }
    }
}