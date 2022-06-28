using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge
{
    public class BillingDetails    {
        [JsonProperty("address")]
        public Address Address { get; set; } 

        [JsonProperty("email")]
        public string Email { get; set; } 

        [JsonProperty("name")]
        public string Name { get; set; } 

        [JsonProperty("phone")]
        public string Phone { get; set; } 
    }
}