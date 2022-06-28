using Highstreetly.Infrastructure.ChargeBee.PlanCreated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface ICreatePlan : ICommand
    {
        PlanCreate PlanCreate { get; set; }
    }
}