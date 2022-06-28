using System;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionReactivated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class ReactivateUserSubscription : IReactivateUserSubscription
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public SubscriptionReactivate SubscriptionReactivate { get; set; }
    }
}