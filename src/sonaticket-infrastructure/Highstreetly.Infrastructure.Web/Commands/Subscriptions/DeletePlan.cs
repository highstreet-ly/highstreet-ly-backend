using System;
using Highstreetly.Infrastructure.ChargeBee.PlanDeleted;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class DeletePlan : IDeletePlan
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public PlanDelete PlanDelete { get; set; }
    }
}