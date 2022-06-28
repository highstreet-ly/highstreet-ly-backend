using Newtonsoft.Json;

namespace Highstreetly.Management.Resources
{
    public class OpeningTime
    {
        [JsonProperty("open")]
        public string Open { get; set; }

        [JsonProperty("close")]
        public string Close { get; set; }
    }
}