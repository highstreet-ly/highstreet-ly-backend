using System;
using Highstreetly.Infrastructure.Messaging;

namespace Highstreetly.Infrastructure.Commands
{
    public interface ICancelTicketReservation : ITicketTypeAvailabilityCommand
    {
         Guid ReservationId { get; set; }
    }
}