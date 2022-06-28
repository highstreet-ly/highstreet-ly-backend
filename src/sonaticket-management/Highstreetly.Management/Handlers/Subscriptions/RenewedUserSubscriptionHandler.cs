using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class RenewedUserSubscriptionHandler : IConsumer<IRenewedUserSubscription>
    {
        private readonly ILogger<RenewedUserSubscriptionHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public RenewedUserSubscriptionHandler(ILogger<RenewedUserSubscriptionHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<IRenewedUserSubscription> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            var incomingSubscription = context.Message.SubscriptionRenew.Content.Subscription;

            var subscription = _managementDbContext.Subscriptions.Single(x =>
                x.IntegrationId == incomingSubscription.Id);

            subscription.CurrentTermStart = incomingSubscription.CurrentTermStart;
            subscription.CurrentTermEnd = incomingSubscription.CurrentTermEnd;

            await _managementDbContext.SaveChangesAsync();
        }
    }
}