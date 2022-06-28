using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.AddOnCreated
{
    public class AddOnCreate
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("occurred_at")]
        public int OccurredAt { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("api_version")]
        public string ApiVersion { get; set; }

        [JsonPropertyName("content")]
        public Content Content { get; set; }

        [JsonPropertyName("event_type")]
        public string EventType { get; set; }

        [JsonPropertyName("webhook_status")]
        public string WebhookStatus { get; set; }
    }
}