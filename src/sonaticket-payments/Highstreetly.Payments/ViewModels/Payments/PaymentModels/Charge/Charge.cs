using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge
{
    public class Charge    {
        [JsonProperty("id")]
        public string Id { get; set; } 

        [JsonProperty("object")]
        public string Object { get; set; } 

        [JsonProperty("amount")]
        public int? Amount { get; set; } 

        [JsonProperty("amount_captured")]
        public int? AmountCaptured { get; set; } 

        [JsonProperty("amount_refunded")]
        public int? AmountRefunded { get; set; } 

        [JsonProperty("application")]
        public string Application { get; set; } 

        [JsonProperty("application_fee")]
        public string ApplicationFee { get; set; } 

        [JsonProperty("application_fee_amount")]
        public int? ApplicationFeeAmount { get; set; } 

        [JsonProperty("balance_transaction")]
        public string BalanceTransaction { get; set; } 

        [JsonProperty("billing_details")]
        public BillingDetails BillingDetails { get; set; } 

        [JsonProperty("calculated_statement_descriptor")]
        public string CalculatedStatementDescriptor { get; set; } 

        [JsonProperty("captured")]
        public bool Captured { get; set; } 

        [JsonProperty("created")]
        public int Created { get; set; } 

        [JsonProperty("currency")]
        public string Currency { get; set; } 

        [JsonProperty("customer")]
        public object Customer { get; set; } 

        [JsonProperty("description")]
        public string Description { get; set; } 

        [JsonProperty("destination")]
        public string Destination { get; set; } 

        [JsonProperty("dispute")]
        public object Dispute { get; set; } 

        [JsonProperty("disputed")]
        public bool Disputed { get; set; } 

        [JsonProperty("failure_code")]
        public string FailureCode { get; set; } 

        [JsonProperty("failure_message")]
        public string FailureMessage { get; set; } 

        [JsonProperty("fraud_details")]
        public FraudDetails FraudDetails { get; set; } 

        [JsonProperty("invoice")]
        public object Invoice { get; set; } 

        [JsonProperty("livemode")]
        public bool Livemode { get; set; } 

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; } 

        [JsonProperty("on_behalf_of")]
        public object OnBehalfOf { get; set; } 

        [JsonProperty("order")]
        public object Order { get; set; } 

        [JsonProperty("outcome")]
        public Outcome Outcome { get; set; } 

        [JsonProperty("paid")]
        public bool Paid { get; set; } 

        [JsonProperty("payment_intent")]
        public string PaymentIntent { get; set; } 

        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; } 

        [JsonProperty("payment_method_details")]
        public PaymentMethodDetails PaymentMethodDetails { get; set; } 

        [JsonProperty("receipt_email")]
        public object ReceiptEmail { get; set; } 

        [JsonProperty("receipt_number")]
        public object ReceiptNumber { get; set; } 

        [JsonProperty("receipt_url")]
        public string ReceiptUrl { get; set; } 

        [JsonProperty("refunded")]
        public bool Refunded { get; set; } 

        [JsonProperty("refunds")]
        public Refunds Refunds { get; set; } 

        [JsonProperty("review")]
        public object Review { get; set; } 

        [JsonProperty("shipping")]
        public object Shipping { get; set; } 

        [JsonProperty("source")]
        public object Source { get; set; } 

        [JsonProperty("source_transfer")]
        public object SourceTransfer { get; set; } 

        [JsonProperty("statement_descriptor")]
        public object StatementDescriptor { get; set; } 

        [JsonProperty("statement_descriptor_suffix")]
        public object StatementDescriptorSuffix { get; set; } 

        [JsonProperty("status")]
        public string Status { get; set; } 

        [JsonProperty("transfer")]
        public string Transfer { get; set; } 

        [JsonProperty("transfer_data")]
        public TransferData TransferData { get; set; } 

        [JsonProperty("transfer_group")]
        public string TransferGroup { get; set; } 
    }
}