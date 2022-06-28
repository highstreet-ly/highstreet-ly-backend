using System;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionCreated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class CreateUserSubscription : ICreateUserSubscription
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public SubscriptionCreate SubscriptionCreate { get; set; }
    }
}
