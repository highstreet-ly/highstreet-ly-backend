using System;
using Highstreetly.Infrastructure.Messaging;

namespace Highstreetly.Infrastructure.Commands
{
    public interface ICommitTicketReservation : ITicketTypeAvailabilityCommand
    {
         Guid ReservationId { get; set; }
    }
}