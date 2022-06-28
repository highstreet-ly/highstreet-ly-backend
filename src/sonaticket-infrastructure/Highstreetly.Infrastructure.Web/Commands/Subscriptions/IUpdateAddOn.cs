using Highstreetly.Infrastructure.ChargeBee.AddOnUpdated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface IUpdateAddOn : ICommand
    {
        AddOnUpdate AddOnUpdate { get; set; }
    }
}