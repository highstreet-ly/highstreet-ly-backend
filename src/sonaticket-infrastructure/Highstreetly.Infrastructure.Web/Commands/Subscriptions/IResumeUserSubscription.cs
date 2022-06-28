using Highstreetly.Infrastructure.ChargeBee.SubscriptionResumed;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface IResumeUserSubscription : ICommand
    {
        SubscriptionResume SubscriptionResume { get; set; }
    }
}