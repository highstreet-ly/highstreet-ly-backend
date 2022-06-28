using Newtonsoft.Json;

namespace Highstreetly.Reservations.Distance
{
    public partial class Element
    {
        [JsonProperty("distance")]
        public Distance Distance { get; set; }

        [JsonProperty("duration")]
        public Distance Duration { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}