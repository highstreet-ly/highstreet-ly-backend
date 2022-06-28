using System;
using System.Collections.Generic;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace Highstreetly.Infrastructure.Requests
{
    [JsonObject(Title = "product-extra-group")]
    [RequestScope(Scope = Scopes.TicketReservationsApi)]
    [Serializable]
    public class ProductExtraGroup
    {
        public ProductExtraGroup()
        {
            ProductExtras = new Relationship<List<ProductExtra>>();
        }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("product-extras")]
        public Relationship<List<ProductExtra>> ProductExtras { get; set; }
        
        [JsonProperty("max-selectable")]
        public int MaxSelectable { get; set; }
        
        [JsonProperty("min-selectable")]
        public int MinSelectable { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty(propertyName: "sort-order")]
        public int? SortOrder { get; set; }
    }
}