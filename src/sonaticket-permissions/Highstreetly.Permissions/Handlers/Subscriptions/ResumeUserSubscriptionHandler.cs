using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using Highstreetly.Permissions.Resources;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class ResumeUserSubscriptionHandler : IConsumer<IResumeUserSubscription>
    {
        private readonly ILogger<ResumeUserSubscriptionHandler> _logger;
        private readonly PermissionsDbContext _permissionsDbContext;

        public ResumeUserSubscriptionHandler(
            ILogger<ResumeUserSubscriptionHandler> logger,
            PermissionsDbContext permissionsDbContext)
        {
            _logger = logger;
            _permissionsDbContext = permissionsDbContext;
        }

        public async Task Consume(
            ConsumeContext<IResumeUserSubscription> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.Id }))
            {
                _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

                var incomingCustomer = context.Message.SubscriptionResume.Content.Customer;

                var user = _permissionsDbContext
                    .Users
                    .Include(x => x.Claims)
                    .Single(x =>
                        x.Email == incomingCustomer.Email);

                var incomingAddons = context.Message.SubscriptionResume.Content.Subscription.Addons;

                if (incomingAddons != null)
                {
                    foreach (var subscriptionAddon in incomingAddons)
                    {
                        user.Claims.Add(new Claim
                        {
                            ClaimType = "feature",
                            ClaimValue = subscriptionAddon.Id
                        });
                    }
                }

                await _permissionsDbContext.SaveChangesAsync();
            }
        }
    }
}