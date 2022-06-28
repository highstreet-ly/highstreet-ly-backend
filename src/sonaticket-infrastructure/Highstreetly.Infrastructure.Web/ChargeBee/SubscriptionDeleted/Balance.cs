using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.SubscriptionDeleted
{
    public class Balance
    {
        [JsonPropertyName("promotional_credits")]
        public int PromotionalCredits { get; set; }

        [JsonPropertyName("excess_payments")]
        public int ExcessPayments { get; set; }

        [JsonPropertyName("refundable_credits")]
        public int RefundableCredits { get; set; }

        [JsonPropertyName("unbilled_charges")]
        public int UnbilledCharges { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("balance_currency_code")]
        public string BalanceCurrencyCode { get; set; }
    }
}