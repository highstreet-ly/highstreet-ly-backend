namespace Highstreetly.Payments.Models.Stripe.Charge
{
    public class Outcome
    {
        public string NetworkStatus { get; set; }

        public object Reason { get; set; }

        public string RiskLevel { get; set; }

        public int RiskScore { get; set; }

        public string SellerMessage { get; set; }

        public string Type { get; set; }
    }
}