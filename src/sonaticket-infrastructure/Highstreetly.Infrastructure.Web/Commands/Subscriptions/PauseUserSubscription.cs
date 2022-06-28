using System;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionPaused;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class PauseUserSubscription : IPauseUserSubscription
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public SubscriptionPause SubscriptionPause { get; set; }
    }
}