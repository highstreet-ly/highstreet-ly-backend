using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.Transfer
{
    public class Transfer
    {
        [JsonProperty("id")]
        public string Id { get; set; } 

        [JsonProperty("object")]
        public string Object { get; set; } 

        [JsonProperty("amount")]
        public int Amount { get; set; } 

        [JsonProperty("amount_reversed")]
        public int AmountReversed { get; set; } 

        [JsonProperty("balance_transaction")]
        public string BalanceTransaction { get; set; } 

        [JsonProperty("created")]
        public int Created { get; set; } 

        [JsonProperty("currency")]
        public string Currency { get; set; } 

        [JsonProperty("description")]
        public object Description { get; set; } 

        [JsonProperty("destination")]
        public string Destination { get; set; } 

        [JsonProperty("destination_payment")]
        public string DestinationPayment { get; set; } 

        [JsonProperty("livemode")]
        public bool Livemode { get; set; } 

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; } 

        [JsonProperty("reversals")]
        public Reversals Reversals { get; set; } 

        [JsonProperty("reversed")]
        public bool Reversed { get; set; } 

        [JsonProperty("source_transaction")]
        public string SourceTransaction { get; set; } 

        [JsonProperty("source_type")]
        public string SourceType { get; set; } 

        [JsonProperty("transfer_group")]
        public string TransferGroup { get; set; } 
    }
}