using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public interface ITicketsReservationCancelled : ISonaticketEvent
    {
         Guid ReservationId { get; set; }
         IEnumerable<TicketQuantity> AvailableTicketsChanged { get; set; }
    }
}