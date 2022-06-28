using System;
using Highstreetly.Infrastructure.ChargeBee.PlanUpdated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class UpdatePlan : IUpdatePlan
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public PlanUpdate PlanUpdate { get; set; }
    }
}