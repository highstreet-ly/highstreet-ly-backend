using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace Highstreetly.Management.Contracts.Requests
{
    [JsonObject(Title = "add-ons")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class AddOn
    {
        public string Type => "add-ons";

        [JsonProperty(propertyName: "id")]
        public Guid Id { get; set; }

        [JsonProperty(propertyName: "plan-id")]
        public Guid PlanId { get; set; }

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

        [JsonProperty("features")]
        public List<Feature> Features { get; set; }
    }
}