using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.SubscriptionCreated
{
    public class LineItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("date_from")]
        public int DateFrom { get; set; }

        [JsonPropertyName("date_to")]
        public int DateTo { get; set; }

        [JsonPropertyName("unit_amount")]
        public int UnitAmount { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("pricing_model")]
        public string PricingModel { get; set; }

        [JsonPropertyName("is_taxed")]
        public bool IsTaxed { get; set; }

        [JsonPropertyName("tax_amount")]
        public int TaxAmount { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("subscription_id")]
        public string SubscriptionId { get; set; }

        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("entity_type")]
        public string EntityType { get; set; }

        [JsonPropertyName("entity_id")]
        public string EntityId { get; set; }

        [JsonPropertyName("entity_description")]
        public string EntityDescription { get; set; }

        [JsonPropertyName("tax_exempt_reason")]
        public string TaxExemptReason { get; set; }

        [JsonPropertyName("discount_amount")]
        public int DiscountAmount { get; set; }

        [JsonPropertyName("item_level_discount_amount")]
        public int ItemLevelDiscountAmount { get; set; }
    }
}