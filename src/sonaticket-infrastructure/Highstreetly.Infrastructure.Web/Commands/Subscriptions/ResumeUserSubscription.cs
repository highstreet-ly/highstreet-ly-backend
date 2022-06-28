using System;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionResumed;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class ResumeUserSubscription : IResumeUserSubscription
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public SubscriptionResume SubscriptionResume { get; set; }
    }
}