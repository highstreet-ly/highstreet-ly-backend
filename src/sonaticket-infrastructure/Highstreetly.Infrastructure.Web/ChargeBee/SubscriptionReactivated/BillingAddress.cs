using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.SubscriptionReactivated
{
    public class BillingAddress
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("validation_status")]
        public string ValidationStatus { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }
    }
}