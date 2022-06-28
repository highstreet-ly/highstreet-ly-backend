using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge
{
    public class Card    {
        [JsonProperty("brand")]
        public string Brand { get; set; } 

        [JsonProperty("checks")]
        public Checks Checks { get; set; } 

        [JsonProperty("country")]
        public string Country { get; set; } 

        [JsonProperty("exp_month")]
        public int ExpMonth { get; set; } 

        [JsonProperty("exp_year")]
        public int ExpYear { get; set; } 

        [JsonProperty("fingerprint")]
        public string Fingerprint { get; set; } 

        [JsonProperty("funding")]
        public string Funding { get; set; } 

        [JsonProperty("installments")]
        public object Installments { get; set; } 

        [JsonProperty("last4")]
        public string Last4 { get; set; } 

        [JsonProperty("network")]
        public string Network { get; set; } 

        [JsonProperty("three_d_secure")]
        public object ThreeDSecure { get; set; } 

        [JsonProperty("wallet")]
        public object Wallet { get; set; } 
    }
}