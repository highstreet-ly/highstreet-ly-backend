using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class TrialEndReminderUserSubscriptionHandler : IConsumer<ITrialEndReminderUserSubscription>
    {
        private readonly ILogger<TrialEndReminderUserSubscriptionHandler> _logger;

        public TrialEndReminderUserSubscriptionHandler(ILogger<TrialEndReminderUserSubscriptionHandler> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ITrialEndReminderUserSubscription> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}