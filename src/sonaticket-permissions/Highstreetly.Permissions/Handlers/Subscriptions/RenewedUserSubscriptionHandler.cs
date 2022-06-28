using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class RenewedUserSubscriptionHandler : IConsumer<IRenewedUserSubscription>
    {
        private readonly ILogger<RenewedUserSubscriptionHandler> _logger;

        public RenewedUserSubscriptionHandler(ILogger<RenewedUserSubscriptionHandler> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IRenewedUserSubscription> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}