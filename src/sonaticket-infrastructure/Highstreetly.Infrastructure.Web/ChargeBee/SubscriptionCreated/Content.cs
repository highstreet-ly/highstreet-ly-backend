using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.SubscriptionCreated
{
    public class Content
    {
        [JsonPropertyName("subscription")]
        public Subscription Subscription { get; set; }

        [JsonPropertyName("customer")]
        public Customer Customer { get; set; }

        [JsonPropertyName("card")]
        public Card Card { get; set; }

        [JsonPropertyName("invoice")]
        public Invoice Invoice { get; set; }
    }
}