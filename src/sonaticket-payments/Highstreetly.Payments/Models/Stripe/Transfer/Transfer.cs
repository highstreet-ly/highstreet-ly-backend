namespace Highstreetly.Payments.Models.Stripe.Transfer
{
    public class Transfer
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public int Amount { get; set; }

        public int AmountReversed { get; set; }

        public string BalanceTransaction { get; set; }

        public int Created { get; set; }

        public string Currency { get; set; }

        public object Description { get; set; }

        public string Destination { get; set; }

        public string DestinationPayment { get; set; }

        public bool Livemode { get; set; }

        public Metadata Metadata { get; set; }

        public Reversals Reversals { get; set; }

        public bool Reversed { get; set; }

        public string SourceTransaction { get; set; }

        public string SourceType { get; set; }

        public string TransferGroup { get; set; }
    }
}