using System;

namespace Highstreetly.Infrastructure.Commands
{
    /// <summary>
    /// Use this when we dont have a stripe customer object for this user 
    /// This will create a customer object 1st and then charge the customer
    /// We can then save the customer object for later so that the user 
    /// Doesn't need to keep entering their card data
    /// </summary>
    public interface IChargeCustomersCardUsingStripeToken : ICommand
    {
        Guid OrderId { get; set; }
        string Token { get; set; }
        Guid PaymemtId { get; set; }
        string Email { get; set; }
    }
}