using Highstreetly.Infrastructure.ChargeBee.SubscriptionRenewed;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface IRenewedUserSubscription : ICommand
    {
        SubscriptionRenew SubscriptionRenew { get; set; }
    }
}