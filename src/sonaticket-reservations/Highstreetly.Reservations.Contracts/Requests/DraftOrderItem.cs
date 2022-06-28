using System;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Requests;
using Newtonsoft.Json;

namespace Highstreetly.Reservations.Contracts.Requests
{
    [JsonObject(Title = "draft-order-items")]
    [RequestScope(Scope = Scopes.TicketReservationsApi)]
    [RequestService(Service = Services.TicketReservationsApi)]
    public class DraftOrderItem
    {
        public string Type => "draft-order-items";

        [JsonProperty(propertyName: "ticket-type")]
        public Guid TicketType { get; set; }

        [JsonProperty(propertyName: "requested-tickets")]
        public int? RequestedTickets { get; set; }

        [JsonProperty(propertyName: "reserved-tickets")]
        public int? ReservedTickets { get; set; }

        [JsonProperty("ticket")]
        public OrderTicketDetails Ticket { get; set; }
        
        [JsonProperty(propertyName: "id")] 
        public string Id { get; set; }
    }
}