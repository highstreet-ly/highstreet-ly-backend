using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Requests;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace Highstreetly.Management.Contracts.Requests
{
    [JsonObject(Title = "ticket-types")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class TicketType
    {
        public string Type => "ticket-types";

        [JsonProperty(propertyName: "id")]
        public Guid Id { get; set; }

        [JsonProperty(propertyName: "event-instance-id")]
        public Guid EventInstanceId { get; set; }

        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }

        [JsonProperty("normalized-name")]
        public string NormalizedName { get; set; }

        [JsonProperty(propertyName: "event-slug")]
        public string EventSlug { get; set; }

        [JsonProperty(propertyName: "description")]
        public string Description { get; set; }

        [JsonProperty(propertyName: "price")]
        public long Price { get; set; }

        [JsonProperty(propertyName: "quantity")]
        public int? Quantity { get; set; }

        [JsonProperty(propertyName: "available-quantity")]
        public int? AvailableQuantity { get; set; }

        [JsonProperty(propertyName: "schedule-start-date")]
        public DateTime? ScheduleStartDate { get; set; }

        [JsonProperty(propertyName: "schedule-end-date")]
        public DateTime? ScheduleEndDate { get; set; }

        [JsonProperty(propertyName: "tickets-available-version")]
        public int? TicketsAvailabilityVersion { get; set; }

        [JsonProperty(propertyName: "published")]
        public bool Published { get; set; }

        [JsonProperty(propertyName: "sort-order")]
        public int? SortOrder { get; set; }

        [JsonProperty("product-extra-groups")]
        public List<ProductExtraGroup> ProductExtraGroups { get; set; }
    }
}