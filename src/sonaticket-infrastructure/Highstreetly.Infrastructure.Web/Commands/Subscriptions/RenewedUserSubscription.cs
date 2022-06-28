using System;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionRenewed;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class RenewedUserSubscription : IRenewedUserSubscription
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public SubscriptionRenew SubscriptionRenew { get; set; }
    }
}