using Newtonsoft.Json;

namespace Highstreetly.Payments.ViewModels.Payments.PaymentModels.Charge
{
    public class Checks    {
        [JsonProperty("address_line1_check")]
        public string AddressLine1Check { get; set; } 

        [JsonProperty("address_postal_code_check")]
        public string AddressPostalCodeCheck { get; set; } 

        [JsonProperty("cvc_check")]
        public string CvcCheck { get; set; } 
    }
}