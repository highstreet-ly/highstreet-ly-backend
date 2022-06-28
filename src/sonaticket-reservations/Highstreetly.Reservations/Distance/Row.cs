using Newtonsoft.Json;

namespace Highstreetly.Reservations.Distance
{
    public partial class Row
    {
        [JsonProperty("elements")]
        public Element[] Elements { get; set; }
    }
}