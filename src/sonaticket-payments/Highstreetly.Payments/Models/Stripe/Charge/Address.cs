using Newtonsoft.Json;

namespace Highstreetly.Payments.Models.Stripe.Charge
{
    public class Address
    {
        [JsonProperty("city")] public object City { get; set; }

        [JsonProperty("country")] public object Country { get; set; }

        [JsonProperty("line1")] public string Line1 { get; set; }

        [JsonProperty("line2")] public object Line2 { get; set; }

        [JsonProperty("postal_code")] public string PostalCode { get; set; }

        [JsonProperty("state")] public object State { get; set; }
    }
}