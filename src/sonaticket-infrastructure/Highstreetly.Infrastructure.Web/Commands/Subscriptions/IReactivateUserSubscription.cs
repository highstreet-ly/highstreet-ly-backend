using Highstreetly.Infrastructure.ChargeBee.SubscriptionReactivated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface IReactivateUserSubscription : ICommand
    {
        SubscriptionReactivate SubscriptionReactivate { get; set; }
    }
}