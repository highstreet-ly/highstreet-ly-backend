using Newtonsoft.Json;

namespace Highstreetly.Management.Resources
{
    public class OpeningTimes
    {
        public OpeningTimes()
        {
            Monday = new OpeningTime();
            Tuesday = new OpeningTime();
            Wednesday = new OpeningTime();
            Thursday = new OpeningTime();
            Friday = new OpeningTime();
            Saturday = new OpeningTime();
            Sunday = new OpeningTime();
        }

        [JsonProperty("monday")]
        public OpeningTime Monday { get; set; }

        [JsonProperty("tuesday")]
        public OpeningTime Tuesday { get; set; }

        [JsonProperty("wednesday")]
        public OpeningTime Wednesday { get; set; }

        [JsonProperty("thursday")]
        public OpeningTime Thursday { get; set; }

        [JsonProperty("friday")]
        public OpeningTime Friday { get; set; }

        [JsonProperty("saturday")]
        public OpeningTime Saturday { get; set; }

        [JsonProperty("sunday")]
        public OpeningTime Sunday { get; set; }
    }
}