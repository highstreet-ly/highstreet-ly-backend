using Highstreetly.Infrastructure.ChargeBee.SubscriptionPaused;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface IPauseUserSubscription : ICommand
    {
        SubscriptionPause SubscriptionPause { get; set; }
    }
}