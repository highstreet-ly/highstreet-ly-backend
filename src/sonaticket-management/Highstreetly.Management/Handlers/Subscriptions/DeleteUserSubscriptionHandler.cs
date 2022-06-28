using System;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class DeleteUserSubscriptionHandler : IConsumer<IDeleteUserSubscription>
    {
        private readonly ILogger<DeleteUserSubscriptionHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public DeleteUserSubscriptionHandler(ILogger<DeleteUserSubscriptionHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<IDeleteUserSubscription> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");
            var subscription = _managementDbContext.Subscriptions.Single(x =>
                x.IntegrationId == context.Message.SubscriptionDelete.Content.Subscription.Id);

            subscription.CancelledAt = (int)DateTime.UtcNow.Ticks;

            await _managementDbContext.SaveChangesAsync();
        }
    }
}