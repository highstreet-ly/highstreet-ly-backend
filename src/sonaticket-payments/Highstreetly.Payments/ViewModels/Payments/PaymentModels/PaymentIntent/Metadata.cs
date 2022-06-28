using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.PaymentIntent
{
    public class Metadata    {
        [JsonProperty("sona-correlation-id")]
        public string SonaCorrelationId { get; set; } 
    }
}