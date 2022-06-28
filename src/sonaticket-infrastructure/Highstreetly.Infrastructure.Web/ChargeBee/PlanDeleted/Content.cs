using System.Text.Json.Serialization;

namespace Highstreetly.Infrastructure.ChargeBee.PlanDeleted
{
    public class Content
    {
        [JsonPropertyName("plan")]
        public Plan Plan { get; set; }
    }
}