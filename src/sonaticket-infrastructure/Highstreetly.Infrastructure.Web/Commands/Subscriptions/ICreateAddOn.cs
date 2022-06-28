using Highstreetly.Infrastructure.ChargeBee.AddOnCreated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface ICreateAddOn : ICommand
    {
        AddOnCreate AddOnCreate { get; set; }
    }
}