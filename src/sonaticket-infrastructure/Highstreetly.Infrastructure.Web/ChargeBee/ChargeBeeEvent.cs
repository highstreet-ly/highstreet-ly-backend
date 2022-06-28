using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee
{
    public class ChargeBeeEvent
    {
        [JsonPropertyName("event_type")]
        public string EventType { get; set; }
    }
}
