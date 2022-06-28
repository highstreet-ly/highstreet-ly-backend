namespace Highstreetly.Payments.Models.Stripe.Charge
{
    public class BillingDetails
    {
        public Address Address { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
    }
}