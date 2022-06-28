using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Infrastructure.Messaging;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IMakeTicketReservation : ITicketTypeAvailabilityCommand
    {
         Guid ReservationId { get; set; }
         List<TicketQuantity> Tickets { get; set; }
         bool IsStockManaged { get; set; }
    }
}