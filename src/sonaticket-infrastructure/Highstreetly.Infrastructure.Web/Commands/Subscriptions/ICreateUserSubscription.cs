using Highstreetly.Infrastructure.ChargeBee.SubscriptionCreated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface ICreateUserSubscription : ICommand
    {
        SubscriptionCreate SubscriptionCreate { get; set; }
    }
}