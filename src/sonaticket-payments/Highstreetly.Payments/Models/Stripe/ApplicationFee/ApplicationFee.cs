namespace Highstreetly.Payments.Models.Stripe.ApplicationFee
{
    public class ApplicationFee
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public string Account { get; set; }

        public int Amount { get; set; }

        public int AmountRefunded { get; set; }

        public string Application { get; set; }

        public string BalanceTransaction { get; set; }

        public string Charge { get; set; }

        public int Created { get; set; }

        public string Currency { get; set; }

        public bool Livemode { get; set; }

        public string OriginatingTransaction { get; set; }

        public bool Refunded { get; set; }

        public Refunds Refunds { get; set; }
    }
}