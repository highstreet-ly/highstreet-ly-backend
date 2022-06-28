using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.PlanUpdated
{
    public class Content
    {
        [JsonPropertyName("plan")]
        public Plan Plan { get; set; }
    }
}