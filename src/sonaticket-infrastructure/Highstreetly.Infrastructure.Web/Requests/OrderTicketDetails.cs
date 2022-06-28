using System;
using System.Collections.Generic;
using JsonApiSerializer.JsonApi;
using MassTransit;
using Newtonsoft.Json;

namespace Highstreetly.Infrastructure.Requests
{
    [JsonObject(Title = "order-ticket-details")]
    [RequestScope(Scope = Scopes.TicketReservationsApi)]
    public class OrderTicketDetails
    {
        public OrderTicketDetails()
        {
            Id = NewId.NextGuid();
            ProductExtras = new Relationship<List<ProductExtra>>();
            ProductExtraGroups = new Relationship<List<ProductExtraGroup>>();
        }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("event-instance-id")]
        public Guid EventInstanceId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("display-name")]
        public string DisplayName { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("product-extra-groups")]
        public Relationship<List<ProductExtraGroup>> ProductExtraGroups { get; set; }

        [JsonProperty("product-extras")]
        public Relationship<List<ProductExtra>> ProductExtras { get; set; }
    }
}
