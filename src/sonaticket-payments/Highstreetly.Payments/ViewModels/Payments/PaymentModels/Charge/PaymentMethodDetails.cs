using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge
{
    public class PaymentMethodDetails    {
        [JsonProperty("card")]
        public Card Card { get; set; } 

        [JsonProperty("type")]
        public string Type { get; set; } 
    }
}