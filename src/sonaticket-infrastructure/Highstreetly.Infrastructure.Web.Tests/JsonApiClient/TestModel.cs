using System;
using Newtonsoft.Json;

namespace Highstreetly.Infrastructure.Web.Tests.JsonApiClient
{
    [JsonObject(Title = "test-models")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class TestModel
    {
        public string Type => "test-models";

        [JsonProperty(propertyName: "id")]
        public Guid Id { get; set; }
    }
}