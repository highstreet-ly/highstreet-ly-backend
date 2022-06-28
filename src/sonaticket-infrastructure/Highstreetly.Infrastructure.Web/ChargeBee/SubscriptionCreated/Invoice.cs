using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.SubscriptionCreated
{
    public class Invoice
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; }

        [JsonPropertyName("subscription_id")]
        public string SubscriptionId { get; set; }

        [JsonPropertyName("recurring")]
        public bool Recurring { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("price_type")]
        public string PriceType { get; set; }

        [JsonPropertyName("date")]
        public int Date { get; set; }

        [JsonPropertyName("due_date")]
        public int DueDate { get; set; }

        [JsonPropertyName("net_term_days")]
        public int NetTermDays { get; set; }

        [JsonPropertyName("exchange_rate")]
        public double ExchangeRate { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("amount_paid")]
        public int AmountPaid { get; set; }

        [JsonPropertyName("amount_adjusted")]
        public int AmountAdjusted { get; set; }

        [JsonPropertyName("write_off_amount")]
        public int WriteOffAmount { get; set; }

        [JsonPropertyName("credits_applied")]
        public int CreditsApplied { get; set; }

        [JsonPropertyName("amount_due")]
        public int AmountDue { get; set; }

        [JsonPropertyName("paid_at")]
        public int PaidAt { get; set; }

        [JsonPropertyName("updated_at")]
        public int UpdatedAt { get; set; }

        [JsonPropertyName("resource_version")]
        public long ResourceVersion { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("first_invoice")]
        public bool FirstInvoice { get; set; }

        [JsonPropertyName("amount_to_collect")]
        public int AmountToCollect { get; set; }

        [JsonPropertyName("round_off_amount")]
        public int RoundOffAmount { get; set; }

        [JsonPropertyName("new_sales_amount")]
        public int NewSalesAmount { get; set; }

        [JsonPropertyName("has_advance_charges")]
        public bool HasAdvanceCharges { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("base_currency_code")]
        public string BaseCurrencyCode { get; set; }

        [JsonPropertyName("is_gifted")]
        public bool IsGifted { get; set; }

        [JsonPropertyName("term_finalized")]
        public bool TermFinalized { get; set; }

        [JsonPropertyName("tax")]
        public int Tax { get; set; }

        [JsonPropertyName("line_items")]
        public List<LineItem> LineItems { get; set; }

        [JsonPropertyName("sub_total")]
        public int SubTotal { get; set; }

        [JsonPropertyName("linked_payments")]
        public List<object> LinkedPayments { get; set; }

        [JsonPropertyName("dunning_attempts")]
        public List<object> DunningAttempts { get; set; }

        [JsonPropertyName("applied_credits")]
        public List<object> AppliedCredits { get; set; }

        [JsonPropertyName("adjustment_credit_notes")]
        public List<object> AdjustmentCreditNotes { get; set; }

        [JsonPropertyName("issued_credit_notes")]
        public List<object> IssuedCreditNotes { get; set; }

        [JsonPropertyName("linked_orders")]
        public List<object> LinkedOrders { get; set; }

        [JsonPropertyName("billing_address")]
        public BillingAddress BillingAddress { get; set; }

        [JsonPropertyName("shipping_address")]
        public ShippingAddress ShippingAddress { get; set; }
    }
}