using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.AddOnCreated
{
    public class Content
    {
        [JsonPropertyName("addon")]
        public Addon Addon { get; set; }
    }
}