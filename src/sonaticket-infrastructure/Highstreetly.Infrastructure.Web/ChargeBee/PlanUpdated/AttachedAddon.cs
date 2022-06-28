using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.PlanUpdated
{
    public class AttachedAddon
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }
    }
}