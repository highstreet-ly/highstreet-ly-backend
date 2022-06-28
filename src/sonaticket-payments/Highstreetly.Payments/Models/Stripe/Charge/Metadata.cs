namespace Highstreetly.Payments.Models.Stripe.Charge
{
    public class Metadata
    {
        public string SonaCorrelationId { get; set; }
        public string SonaOrgId { get; set; }
        public string OrderId { get; set; }
    }
}