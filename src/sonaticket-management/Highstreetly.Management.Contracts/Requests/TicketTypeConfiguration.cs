using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Requests;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace Highstreetly.Management.Contracts.Requests
{
    [JsonObject(Title = "ticket-type-configurations")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class TicketTypeConfiguration
    {
        public string Type => "ticket-type-configurations";

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("event-instance-id")]
        public Guid EventInstanceId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("normalized-name")]
        public string NormalizedName { get; set; }

        [JsonProperty("event-slug")]
        public string EventSlug { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public long? Price { get; set; }

        [JsonProperty("free-tier")]
        public bool FreeTier { get; set; }

        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        [JsonProperty("schedule-start-date")]
        public DateTime? ScheduleStartDate { get; set; }

        [JsonProperty("schedule-end-date")]
        public DateTime? ScheduleEndDate { get; set; }

        [JsonProperty("tickets-availability-version")]
        public int? TicketsAvailabilityVersion { get; set; }

        [JsonProperty("available-quantity")]
        public int? AvailableQuantity { get; set; }

        [JsonProperty("main-image-id")]
        public string MainImageId { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("is-published")]
        public bool IsPublished { get; set; }

        [JsonProperty(propertyName: "sort-order")]
        public int? SortOrder { get; set; }

        [JsonProperty("product-extra-groups")]
        public List<ProductExtraGroup> ProductExtraGroups { get; set; }
    }
}