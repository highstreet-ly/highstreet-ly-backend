using System;
using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Management.Contracts.Requests
{
    [JsonObject(Title = "features")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class Feature
    {
        public string Type => "features";

        [JsonProperty(propertyName: "id")]
        public Guid Id { get; set; }

        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }

        [JsonProperty("normalized-name")]
        public string NormalizedName { get; set; }

        [JsonProperty(propertyName: "description")]
        public string Description { get; set; }

        [JsonProperty(propertyName: "claim-value")]
        public string ClaimValue { get; set; }

        [JsonProperty(propertyName: "deleted")]
        public bool? Deleted { get; set; }
    }
}