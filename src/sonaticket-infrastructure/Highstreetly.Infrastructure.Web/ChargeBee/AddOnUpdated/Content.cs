using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.AddOnUpdated
{
    public class Content
    {
        [JsonPropertyName("addon")]
        public Addon Addon { get; set; }
    }
}