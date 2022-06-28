using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Management.Contracts.Requests
{
    [JsonObject(Title = "order-tickets")]
    [RequestScope(Scope = Scopes.TicketManagementApi)]
    [RequestService(Service = Services.TicketManagementApi)]
    public class OrderTicket
    {
        public string Type => "order-tickets";

        [JsonProperty(propertyName: "id")]
        public string Id { get; set; }

        [JsonProperty("position")]
        public int? Position { get; set; }

        [JsonProperty("ticket-name")]
        public string TicketName { get; set; }
    }
}