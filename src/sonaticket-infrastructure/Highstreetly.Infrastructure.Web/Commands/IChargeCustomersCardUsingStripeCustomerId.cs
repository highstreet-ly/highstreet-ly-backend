using System;

namespace Highstreetly.Infrastructure.Commands
{
    /// <summary>
    /// Charge a customer we already know about - saved card details @stripe
    /// </summary>
    public interface IChargeCustomersCardUsingStripeCustomerId : ICommand
    {
        Guid OrderId { get; set; }
        string StripeCustomerId { get; set; }
    }
}