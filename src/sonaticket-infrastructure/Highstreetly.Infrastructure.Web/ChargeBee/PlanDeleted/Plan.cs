using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.PlanDeleted
{
    public class Plan
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("period")]
        public int Period { get; set; }

        [JsonPropertyName("period_unit")]
        public string PeriodUnit { get; set; }

        [JsonPropertyName("charge_model")]
        public string ChargeModel { get; set; }

        [JsonPropertyName("free_quantity")]
        public int FreeQuantity { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("enabled_in_hosted_pages")]
        public bool EnabledInHostedPages { get; set; }

        [JsonPropertyName("enabled_in_portal")]
        public bool EnabledInPortal { get; set; }

        [JsonPropertyName("updated_at")]
        public int UpdatedAt { get; set; }

        [JsonPropertyName("resource_version")]
        public long ResourceVersion { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("taxable")]
        public bool Taxable { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }
    }
}
