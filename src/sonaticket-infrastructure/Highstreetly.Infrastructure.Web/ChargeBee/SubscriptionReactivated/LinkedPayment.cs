using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.SubscriptionReactivated
{
    public class LinkedPayment
    {
        [JsonPropertyName("txn_id")]
        public string TxnId { get; set; }

        [JsonPropertyName("applied_amount")]
        public int AppliedAmount { get; set; }

        [JsonPropertyName("applied_at")]
        public int AppliedAt { get; set; }

        [JsonPropertyName("txn_status")]
        public string TxnStatus { get; set; }

        [JsonPropertyName("txn_date")]
        public int TxnDate { get; set; }

        [JsonPropertyName("txn_amount")]
        public int TxnAmount { get; set; }
    }
}