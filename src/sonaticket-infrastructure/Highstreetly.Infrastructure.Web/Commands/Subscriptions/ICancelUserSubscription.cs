using Highstreetly.Infrastructure.ChargeBee.SubscriptionCancelled;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface ICancelUserSubscription : ICommand
    {
        SubscriptionCancel SubscriptionCancel { get; set; }
    }
}