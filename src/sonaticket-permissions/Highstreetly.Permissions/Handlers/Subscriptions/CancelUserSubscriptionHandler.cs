using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class CancelUserSubscriptionHandler : IConsumer<ICancelUserSubscription>
    {
        private readonly ILogger<CancelUserSubscriptionHandler> _logger;
        private readonly PermissionsDbContext _permissionsDbContext;

        public CancelUserSubscriptionHandler(
            ILogger<CancelUserSubscriptionHandler> logger,
            PermissionsDbContext permissionsDbContext)
        {
            _logger = logger;
            _permissionsDbContext = permissionsDbContext;
        }

        public async Task Consume(
            ConsumeContext<ICancelUserSubscription> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.Id }))
            {
                _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");
                var incomingCustomer = context.Message.SubscriptionCancel.Content.Customer;

                var user = _permissionsDbContext
                    .Users
                    .Include(x => x.Claims)
                    .Single(x =>
                        x.Email == incomingCustomer.Email);

                var incomingAddons = context.Message.SubscriptionCancel.Content.Subscription.Addons;

                if (incomingAddons != null)
                {
                    foreach (var addon in incomingAddons)
                    {
                        var toRemove =
                            user.Claims.Where(x => x.ClaimType == "feature" && x.ClaimValue == addon.Id);

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