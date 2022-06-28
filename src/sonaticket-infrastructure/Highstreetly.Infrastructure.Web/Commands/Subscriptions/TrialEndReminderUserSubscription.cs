using System;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionTrialEndReminder;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class TrialEndReminderUserSubscription : ITrialEndReminderUserSubscription
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public SubscriptionTrialEndReminder SubscriptionTrialEndReminder { get; set; }
    }
}