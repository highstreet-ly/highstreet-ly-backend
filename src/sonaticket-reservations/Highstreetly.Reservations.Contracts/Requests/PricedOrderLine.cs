using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Requests;
using Newtonsoft.Json;

namespace Highstreetly.Reservations.Contracts.Requests
{
    [JsonObject(Title = "priced-order-line")]
    [RequestScope(Scope = Scopes.TicketReservationsApi)]
    [RequestService(Service = Services.TicketReservationsApi)]
    public class PricedOrderLine
    {
        [JsonProperty(propertyName: "id")] 
        public string Id { get; set; }

        [JsonProperty(propertyName: "type")]
        public string Type => "priced-order-lines";

        [JsonProperty(propertyName: "position")] 
        public int? Position { get; set; }

        [JsonProperty(propertyName: "description")] 
        public string Description { get; set; }

        [JsonProperty(propertyName: "unit-price")]
        public long? UnitPrice { get; set; }

        [JsonProperty(propertyName: "quantity")] 
        public int? Quantity { get; set; }

        [JsonProperty(propertyName: "line-total")] 
        public long? LineTotal { get; set; }

        [JsonProperty("product-extras")]
        public List<ProductExtra> ProductExtras { get; set; }

        [JsonProperty(propertyName: "ticket-type")]
        public Guid TicketType { get; set; }
    }
}