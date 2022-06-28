using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public interface IOrderPartiallyReserved : ISonaticketEvent
    {
         DateTime ReservationExpiration { get; set; }
         IEnumerable<TicketQuantity> Tickets { get; set; }
    }
}