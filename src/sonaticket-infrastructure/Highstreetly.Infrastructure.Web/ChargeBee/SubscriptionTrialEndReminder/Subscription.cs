using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.SubscriptionTrialEndReminder
{
    public class Subscription
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; }

        [JsonPropertyName("plan_id")]
        public string PlanId { get; set; }

        [JsonPropertyName("plan_quantity")]
        public int PlanQuantity { get; set; }

        [JsonPropertyName("plan_unit_price")]
        public int PlanUnitPrice { get; set; }

        [JsonPropertyName("billing_period")]
        public int BillingPeriod { get; set; }

        [JsonPropertyName("billing_period_unit")]
        public string BillingPeriodUnit { get; set; }

        [JsonPropertyName("plan_free_quantity")]
        public int PlanFreeQuantity { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("trial_start")]
        public int TrialStart { get; set; }

        [JsonPropertyName("trial_end")]
        public int TrialEnd { get; set; }

        [JsonPropertyName("next_billing_at")]
        public int NextBillingAt { get; set; }

        [JsonPropertyName("created_at")]
        public int CreatedAt { get; set; }

        [JsonPropertyName("started_at")]
        public int StartedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public int UpdatedAt { get; set; }

        [JsonPropertyName("has_scheduled_changes")]
        public bool HasScheduledChanges { get; set; }

        [JsonPropertyName("resource_version")]
        public long ResourceVersion { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("addons")]
        public List<Addon> Addons { get; set; }

        [JsonPropertyName("due_invoices_count")]
        public int DueInvoicesCount { get; set; }
    }
}