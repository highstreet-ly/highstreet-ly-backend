using System;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionDeleted;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class DeleteUserSubscription : IDeleteUserSubscription
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public SubscriptionDelete SubscriptionDelete { get; set; }
    }
}