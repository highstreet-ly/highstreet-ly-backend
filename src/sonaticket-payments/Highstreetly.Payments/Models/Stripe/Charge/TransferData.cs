namespace Highstreetly.Payments.Models.Stripe.Charge
{
    public class TransferData
    {
        public object Amount { get; set; }

        public string Destination { get; set; }
    }
}