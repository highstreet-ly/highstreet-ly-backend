namespace Highstreetly.Payments.Models.Stripe.Charge
{
    public class PaymentMethodDetails
    {
        public Card Card { get; set; }

        public string Type { get; set; }
    }
}