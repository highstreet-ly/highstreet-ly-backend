using Newtonsoft.Json;

namespace Highstreetly.Reservations.Distance
{
    public partial class DistanceModel
    {
        [JsonProperty("destination_addresses")]
        public string[] DestinationAddresses { get; set; }

        [JsonProperty("origin_addresses")]
        public string[] OriginAddresses { get; set; }

        [JsonProperty("rows")]
        public Row[] Rows { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        public static DistanceModel FromJson(string json) => JsonConvert.DeserializeObject<DistanceModel>(json, Converter.Settings);
    }
}