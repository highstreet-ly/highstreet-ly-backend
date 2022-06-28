using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.SubscriptionDeleted
{
    public class Card
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("gateway")]
        public string Gateway { get; set; }

        [JsonPropertyName("gateway_account_id")]
        public string GatewayAccountId { get; set; }

        [JsonPropertyName("iin")]
        public string Iin { get; set; }

        [JsonPropertyName("last4")]
        public string Last4 { get; set; }

        [JsonPropertyName("card_type")]
        public string CardType { get; set; }

        [JsonPropertyName("funding_type")]
        public string FundingType { get; set; }

        [JsonPropertyName("expiry_month")]
        public int ExpiryMonth { get; set; }

        [JsonPropertyName("expiry_year")]
        public int ExpiryYear { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("masked_number")]
        public string MaskedNumber { get; set; }

        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; }

        [JsonPropertyName("payment_source_id")]
        public string PaymentSourceId { get; set; }
    }
}