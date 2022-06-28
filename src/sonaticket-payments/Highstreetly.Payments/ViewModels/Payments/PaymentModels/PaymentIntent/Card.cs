using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.PaymentIntent
{
    public class Card    {
        [JsonProperty("installments")]
        public object Installments { get; set; } 

        [JsonProperty("network")]
        public object Network { get; set; } 

        [JsonProperty("request_three_d_secure")]
        public string RequestThreeDSecure { get; set; } 
    }
}