namespace Highstreetly.Payments.Models.Stripe.Charge
{
    public class Card
    {
        public string Brand { get; set; }

        public Checks Checks { get; set; }

        public string Country { get; set; }

        public int ExpMonth { get; set; }

        public int ExpYear { get; set; }

        public string Fingerprint { get; set; }

        public string Funding { get; set; }

        public object Installments { get; set; }

        public string Last4 { get; set; }

        public string Network { get; set; }

        public object ThreeDSecure { get; set; }

        public object Wallet { get; set; }
    }
}