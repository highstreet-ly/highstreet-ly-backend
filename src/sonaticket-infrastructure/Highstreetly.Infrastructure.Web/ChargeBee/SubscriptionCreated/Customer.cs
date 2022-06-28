using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.SubscriptionCreated
{
    public class Customer
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("auto_collection")]
        public string AutoCollection { get; set; }

        [JsonPropertyName("net_term_days")]
        public int NetTermDays { get; set; }

        [JsonPropertyName("allow_direct_debit")]
        public bool AllowDirectDebit { get; set; }

        [JsonPropertyName("created_at")]
        public int CreatedAt { get; set; }

        [JsonPropertyName("created_from_ip")]
        public string CreatedFromIp { get; set; }

        [JsonPropertyName("taxability")]
        public string Taxability { get; set; }

        [JsonPropertyName("updated_at")]
        public int UpdatedAt { get; set; }

        [JsonPropertyName("pii_cleared")]
        public string PiiCleared { get; set; }

        [JsonPropertyName("resource_version")]
        public long ResourceVersion { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("billing_address")]
        public BillingAddress BillingAddress { get; set; }

        [JsonPropertyName("card_status")]
        public string CardStatus { get; set; }

        [JsonPropertyName("promotional_credits")]
        public int PromotionalCredits { get; set; }

        [JsonPropertyName("refundable_credits")]
        public int RefundableCredits { get; set; }

        [JsonPropertyName("excess_payments")]
        public int ExcessPayments { get; set; }

        [JsonPropertyName("unbilled_charges")]
        public int UnbilledCharges { get; set; }

        [JsonPropertyName("preferred_currency_code")]
        public string PreferredCurrencyCode { get; set; }

        [JsonPropertyName("mrr")]
        public int Mrr { get; set; }

        [JsonPropertyName("primary_payment_source_id")]
        public string PrimaryPaymentSourceId { get; set; }

        [JsonPropertyName("payment_method")]
        public PaymentMethod PaymentMethod { get; set; }
    }
}