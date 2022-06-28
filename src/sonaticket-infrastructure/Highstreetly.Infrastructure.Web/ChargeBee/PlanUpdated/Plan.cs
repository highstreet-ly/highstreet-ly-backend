using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.PlanUpdated
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Plan
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("invoice_name")]
        public string InvoiceName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("period")]
        public int Period { get; set; }

        [JsonPropertyName("period_unit")]
        public string PeriodUnit { get; set; }

        [JsonPropertyName("pricing_model")]
        public string PricingModel { get; set; }

        [JsonPropertyName("free_quantity")]
        public int FreeQuantity { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonPropertyName("enabled_in_hosted_pages")]
        public bool EnabledInHostedPages { get; set; }

        [JsonPropertyName("enabled_in_portal")]
        public bool EnabledInPortal { get; set; }

        [JsonPropertyName("addon_applicability")]
        public string AddonApplicability { get; set; }

        [JsonPropertyName("is_shippable")]
        public bool IsShippable { get; set; }

        [JsonPropertyName("updated_at")]
        public int UpdatedAt { get; set; }

        [JsonPropertyName("giftable")]
        public bool Giftable { get; set; }

        [JsonPropertyName("resource_version")]
        public long ResourceVersion { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("charge_model")]
        public string ChargeModel { get; set; }

        [JsonPropertyName("taxable")]
        public bool Taxable { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("show_description_in_invoices")]
        public bool ShowDescriptionInInvoices { get; set; }

        [JsonPropertyName("show_description_in_quotes")]
        public bool ShowDescriptionInQuotes { get; set; }

        [JsonPropertyName("attached_addons")]
        public List<AttachedAddon> AttachedAddons { get; set; }
    }
}
