using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Reservations.Contracts.Requests
{
    [JsonObject(Title = "order-tickets")]
    [RequestScope(Scope = Scopes.TicketReservationsApi)]
    [RequestService(Service = Services.TicketReservationsApi)]
    public class OrderTickets
    {
        public string Type => "order-tickets";
        [JsonProperty(propertyName: "id")] public Guid Id { get; set; }
        [JsonProperty(propertyName: "assignments-id")] public Guid AssignmentsId { get; set; }
        [JsonProperty(propertyName: "order-id")] public Guid OrderId { get; set; }
        [JsonProperty(propertyName: "tickets")] public IList<OrderTicket> Tickets { get; set; }
    }
}