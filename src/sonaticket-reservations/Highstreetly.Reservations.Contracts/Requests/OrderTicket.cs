using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Reservations.Contracts.Requests
{
    [JsonObject(Title = "order-ticket")]
    [RequestScope(Scope = Scopes.TicketReservationsApi)]
    [RequestService(Service = Services.TicketReservationsApi)]
    public class OrderTicket
    {
        public string Type => "order-ticket";
        [JsonProperty(propertyName: "id")] public string Id { get; set; }
        [JsonProperty(propertyName: "position")] public int? Position { get; set; }
        [JsonProperty(propertyName: "ticket-name")] public string TicketName { get; set; }
        // [JsonProperty(propertyName: "attendee")] public PersonalInfo Attendee { get; set; }
    }
}