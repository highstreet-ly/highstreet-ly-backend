using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.ApplicationFee
{
    public class ApplicationFee    {
        [JsonProperty("id")]
        public string Id { get; set; } 

        [JsonProperty("object")]
        public string Object { get; set; } 

        [JsonProperty("account")]
        public string Account { get; set; } 

        [JsonProperty("amount")]
        public int Amount { get; set; } 

        [JsonProperty("amount_refunded")]
        public int AmountRefunded { get; set; } 

        [JsonProperty("application")]
        public string Application { get; set; } 

        [JsonProperty("balance_transaction")]
        public string BalanceTransaction { get; set; } 

        [JsonProperty("charge")]
        public string Charge { get; set; } 

        [JsonProperty("created")]
        public int Created { get; set; } 

        [JsonProperty("currency")]
        public string Currency { get; set; } 

        [JsonProperty("livemode")]
        public bool Livemode { get; set; } 

        [JsonProperty("originating_transaction")]
        public string OriginatingTransaction { get; set; } 

        [JsonProperty("refunded")]
        public bool Refunded { get; set; } 

        [JsonProperty("refunds")]
        public Refunds Refunds { get; set; } 
    }
}