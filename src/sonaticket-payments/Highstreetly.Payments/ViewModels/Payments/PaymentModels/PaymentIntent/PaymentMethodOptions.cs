using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.PaymentIntent
{
    public class PaymentMethodOptions    {
        [JsonProperty("card")]
        public Card Card { get; set; } 
    }
}