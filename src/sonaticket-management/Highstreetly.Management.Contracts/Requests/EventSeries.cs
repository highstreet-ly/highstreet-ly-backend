using System;
using Highstreetly.Infrastructure;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace Highstreetly.Management.Contracts.Requests
{
    [JsonObject(Title = "event-series")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class EventSeries
    {
        public string Type => "event-series";

        [JsonProperty(propertyName: "id")]
        public Guid Id { get; set; }

        [JsonProperty(propertyName: "slug")]
        public string Slug { get; set; }

        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }

        [JsonProperty("normalized-name")]
        public string NormalizedName { get; set; }

        [JsonProperty(propertyName: "main-image-id")]
        public string MainImageId { get; set; }

        [JsonProperty(propertyName: "owner-id")]
        public string OwnerId { get; set; }

        [JsonProperty(propertyName: "owner-email")]
        public string OwnerEmail { get; set; }

        [JsonProperty(propertyName: "event-organiser-id")]
        public Guid EventOrganiserId { get; set; }

        public EventOrganiser EventOrganiser { get; set; }

        [JsonProperty("onboarding")]
        public bool Onboarding { get; set; }
    }
}