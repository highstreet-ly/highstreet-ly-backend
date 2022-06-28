using Highstreetly.Infrastructure.ChargeBee.PlanDeleted;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface IDeletePlan : ICommand
    {
        PlanDelete PlanDelete { get; set; }
    }
}