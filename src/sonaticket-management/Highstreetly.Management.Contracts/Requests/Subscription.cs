using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace Highstreetly.Management.Contracts.Requests
{
    [JsonObject(Title = "subscriptions")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class Subscription
    {
        public string Type => "subscriptions";

        [JsonProperty(propertyName: "id")]
        public Guid Id { get; set; }

        [JsonProperty("customer-id")]
        public string CustomerId { get; set; }

        [JsonProperty("user-id")]
        public Guid? UserId { get; set; }

        [JsonProperty("user-email")]
        public string UserEmail { get; set; }

        [JsonProperty("add-ons")]
        public List<AddOn> AddOns { get; set; }

        [JsonProperty("plan")]
        public Plan Plan { get; set; }

        [JsonProperty("cancelled-at")]
        public int? CancelledAt { get; set; }

        [JsonProperty("created-at")]
        public int? CreatedAt { get; set; }
    }
}