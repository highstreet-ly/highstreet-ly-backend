using System;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionCancelled;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class CancelUserSubscription : ICancelUserSubscription
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public SubscriptionCancel SubscriptionCancel { get; set; }
    }
}