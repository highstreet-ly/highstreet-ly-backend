using Newtonsoft.Json;

namespace Highstreetly.Reservations.Distance
{
    public partial class Distance
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }
}