using Highstreetly.Infrastructure.ChargeBee.PlanUpdated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface IUpdatePlan : ICommand
    {
        PlanUpdate PlanUpdate { get; set; }
    }
}