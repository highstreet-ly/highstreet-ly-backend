using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class PauseUserSubscriptionHandler : IConsumer<IPauseUserSubscription>
    {
        private readonly ILogger<PauseUserSubscriptionHandler> _logger;
        private readonly PermissionsDbContext _permissionsDbContext;

        public PauseUserSubscriptionHandler(
            ILogger<PauseUserSubscriptionHandler> logger,
            PermissionsDbContext permissionsDbContext)
        {
            _logger = logger;
            _permissionsDbContext = permissionsDbContext;
        }

        public async Task Consume(
            ConsumeContext<IPauseUserSubscription> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.Id }))
            {
                _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

                var incomingCustomer = context.Message.SubscriptionPause.Content.Customer;

                var user = _permissionsDbContext
                    .Users
                    .Include(x => x.Claims)
                    .Single(x =>
                        x.Email == incomingCustomer.Email);

                var incomingAddons = context.Message.SubscriptionPause.Content.Subscription.Addons;

                if (incomingAddons != null)
                {
                    foreach (var subscriptionAddon in incomingAddons)
                    {
                        var toRemove =
                            user.Claims.Where(x => x.ClaimType == "feature" && x.ClaimValue == subscriptionAddon.Id);

                        foreach (var claim in toRemove)
                        {
                            user.Claims.Remove(claim);
                        }
                    }
                }

                await _permissionsDbContext.SaveChangesAsync();
            }
        }
    }
}