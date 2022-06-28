using System;
using Highstreetly.Infrastructure.ChargeBee.PlanCreated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class CreatePlan : ICreatePlan
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public PlanCreate PlanCreate { get; set; }
    }
}