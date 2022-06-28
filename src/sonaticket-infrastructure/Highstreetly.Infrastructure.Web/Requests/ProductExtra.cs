using System;
using Newtonsoft.Json;

namespace Highstreetly.Infrastructure.Requests
{
    [JsonObject(Title = "product-extra")]
    [RequestScope(Scope = Scopes.TicketReservationsApi)]
    [Serializable]
    public class ProductExtra
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("selected")]
        public bool Selected { get; set; }

        [JsonProperty("item-count")]
        public long ItemCount { get; set; } = 0;
        
        [JsonProperty("reference-product-extra-id")]
        public Guid ReferenceProductExtraId { get; set; }

        [JsonProperty(propertyName: "sort-order")]
        public int? SortOrder { get; set; }
    }
}