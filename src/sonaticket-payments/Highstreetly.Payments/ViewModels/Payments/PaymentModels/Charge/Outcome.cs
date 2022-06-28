using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge
{
    public class Outcome    {
        [JsonProperty("network_status")]
        public string NetworkStatus { get; set; } 

        [JsonProperty("reason")]
        public object Reason { get; set; } 

        [JsonProperty("risk_level")]
        public string RiskLevel { get; set; } 

        [JsonProperty("risk_score")]
        public int RiskScore { get; set; } 

        [JsonProperty("seller_message")]
        public string SellerMessage { get; set; } 

        [JsonProperty("type")]
        public string Type { get; set; } 
    }
}