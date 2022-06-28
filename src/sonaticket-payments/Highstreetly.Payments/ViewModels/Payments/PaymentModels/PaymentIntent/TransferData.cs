using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.PaymentIntent
{
    public class TransferData    {
        [JsonProperty("destination")]
        public string Destination { get; set; } 
    }
}