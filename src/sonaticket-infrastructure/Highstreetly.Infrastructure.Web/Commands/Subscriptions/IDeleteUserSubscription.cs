using Highstreetly.Infrastructure.ChargeBee.SubscriptionDeleted;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface IDeleteUserSubscription : ICommand
    {
        SubscriptionDelete SubscriptionDelete { get; set; }
    }
}