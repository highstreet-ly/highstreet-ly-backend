using System;

namespace Highstreetly.Infrastructure.Commands
{
    /// <summary>
    /// Use this when we don't have a stripe customer object for this user 
    /// This will create a customer object 1st and then charge the customer
    /// We can then save the customer object for later so that the user 
    /// Doesn't need to keep entering their card data
    /// </summary>
    public class ChargeCustomersCardUsingStripeToken : ICommand
    {
        public ChargeCustomersCardUsingStripeToken()
        {
            Id = Guid.NewGuid();
        }

        public Guid OrderId { get; set; }
        public string Token { get; set; }
        public Guid Id { get; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public Guid PaymentId { get; set; }
       public Guid CorrelationId { get; set; }
    }
}