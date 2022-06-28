using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge
{
    public class TransferData    {
        [JsonProperty("amount")]
        public object Amount { get; set; } 

        [JsonProperty("destination")]
        public string Destination { get; set; } 
    }
}