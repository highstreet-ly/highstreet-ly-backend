using Highstreetly.Infrastructure.ChargeBee.SubscriptionTrialEndReminder;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface ITrialEndReminderUserSubscription : ICommand
    {
        SubscriptionTrialEndReminder SubscriptionTrialEndReminder { get; set; }
    }
}