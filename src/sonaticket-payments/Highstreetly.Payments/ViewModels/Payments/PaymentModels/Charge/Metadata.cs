using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge
{
    public class Metadata    {
        [JsonProperty("sona-correlation-id")]
        public string SonaCorrelationId { get; set; } 
        
        [JsonProperty("order-id")]
        public string OrderId { get; set; }

        [JsonProperty("sona-org-id")]
        public string SonaOrgId { get; set; }
    }
}