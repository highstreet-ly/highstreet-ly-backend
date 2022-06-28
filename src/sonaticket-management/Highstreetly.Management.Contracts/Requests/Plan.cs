using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace Highstreetly.Management.Contracts.Requests
{
    [JsonObject(Title = "plans")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class Plan : IHasResourceMetadata
    {
        public string Type => "plans";

        [JsonProperty(propertyName: "id")]
        public Guid Id { get; set; }

        [JsonProperty(propertyName: "integration-id")]
        public string IntegrationId { get; set; }

        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }

        [JsonProperty("normalized-name")]
        public string NormalizedName { get; set; }

        [JsonProperty(propertyName: "pricing-model")]
        public string PricingModel { get; set; }

        [JsonProperty(propertyName: "price")]
        public int Price { get; set; }

        [JsonProperty(propertyName: "status")]
        public string Status { get; set; }

        [JsonProperty(propertyName: "description")]
        public string Description { get; set; }

        [JsonProperty("add-ons")]
        public List<AddOn> AddOns { get; set; }

        [JsonProperty("features")]
        public List<Feature> Features { get; set; }

        [JsonProperty("ticket-types")]
        public List<TicketType> TicketTypes { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        [JsonProperty("period")]
        public int? Period { get; set; }

        [JsonProperty("period-unit")]
        public string PeriodUnit { get; set; }
    }
}