using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface ITicketReservationCommitted : ISonaticketEvent
    {
         Guid ReservationId { get; set; }
    }
}