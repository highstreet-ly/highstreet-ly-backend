using System;

namespace Highstreetly.Infrastructure.Commands
{
    /// <summary>
    /// Charge a customer we already know about - saved card details @stripe
    /// </summary>
    public class ChargeCustomersCardUsingStripeCustomerId : ICommand
    {
        public ChargeCustomersCardUsingStripeCustomerId()
        {
            Id = Guid.NewGuid();
        }

        public Guid OrderId { get; set; }
        public string StripeCustomerId { get; set; }
        public Guid Id { get; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public Guid CorrelationId { get; set; }
    }
}