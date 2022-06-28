using Highstreetly.Infrastructure.ChargeBee.SubscriptionStarted;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface IStartUserSubscription : ICommand
    {
        SubscriptionStart SubscriptionStart { get; set; }
    }
}