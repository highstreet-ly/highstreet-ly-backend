namespace Highstreetly.Payments.Models.Stripe.PaymentIntent
{
    public class Card
    {
        public object Installments { get; set; }

        public object Network { get; set; }

        public string RequestThreeDSecure { get; set; }
    }
}