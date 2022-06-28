using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public interface IOrderReservationCompleted : ISonaticketEvent
    {
        DateTime ReservationExpiration { get; set; }
        IEnumerable<TicketQuantity> Tickets { get; set; }
    }
}