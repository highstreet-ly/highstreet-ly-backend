using System;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionStarted;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class StartUserSubscription : IStartUserSubscription
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public SubscriptionStart SubscriptionStart { get; set; }
    }
}