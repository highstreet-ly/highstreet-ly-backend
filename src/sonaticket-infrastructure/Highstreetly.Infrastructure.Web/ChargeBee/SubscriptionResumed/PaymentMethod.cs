using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.SubscriptionResumed
{
    public class PaymentMethod
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("reference_id")]
        public string ReferenceId { get; set; }

        [JsonPropertyName("gateway")]
        public string Gateway { get; set; }

        [JsonPropertyName("gateway_account_id")]
        public string GatewayAccountId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}