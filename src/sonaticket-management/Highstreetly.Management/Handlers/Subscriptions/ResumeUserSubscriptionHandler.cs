using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class ResumeUserSubscriptionHandler : IConsumer<IResumeUserSubscription>
    {
        private readonly ILogger<ResumeUserSubscriptionHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public ResumeUserSubscriptionHandler(ILogger<ResumeUserSubscriptionHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<IResumeUserSubscription> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");
            var incomingSubscription = context.Message.SubscriptionResume.Content.Subscription;

            var subscription = _managementDbContext.Subscriptions.Single(x =>
                x.IntegrationId == incomingSubscription.Id);

            subscription.CurrentTermStart = incomingSubscription.CurrentTermStart;
            subscription.CurrentTermEnd = incomingSubscription.CurrentTermEnd;

            await _managementDbContext.SaveChangesAsync();
        }
    }
}