using Highstreetly.Infrastructure.ChargeBee.AddOnDeleted;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public interface IDeleteAddOn : ICommand
    {
        AddOnDelete AddOnDelete { get; set; }
    }
}