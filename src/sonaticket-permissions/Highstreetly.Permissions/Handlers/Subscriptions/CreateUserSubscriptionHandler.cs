using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Handlers.Subscriptions
{
    public class CreateUserSubscriptionHandler : IConsumer<ICreateUserSubscription>
    {
        private readonly ILogger<CreateUserSubscriptionHandler> _logger;
        private readonly PermissionsDbContext _permissionsDbContext;

        public CreateUserSubscriptionHandler(ILogger<CreateUserSubscriptionHandler> logger, PermissionsDbContext permissionsDbContext)
        {
            _logger = logger;
            _permissionsDbContext = permissionsDbContext;
        }

        public  Task Consume(ConsumeContext<ICreateUserSubscription> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            return Task.CompletedTask;

            // var user = _permissionsDbContext
            //     .Users
            //     .Include(x=>x.Claims)
            //     .Single(x =>
            //     x.Email == context.Message.SubscriptionCreate.Content.Customer.Email);
            //
            // foreach (var subscriptionAddon in context.Message.SubscriptionCreate.Content.Subscription.Addons)
            // {
            //     user.Claims.Add(new Claim
            //     {
            //         ClaimType = "feature",
            //         ClaimValue = subscriptionAddon.Id
            //     });
            // }

            // await _permissionsDbContext.SaveChangesAsync();
        }
    }
}
