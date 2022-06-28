using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public class TicketsReservationCancelled : ITicketsReservationCancelled
    {
        public Guid ReservationId { get; set; }

        public IEnumerable<TicketQuantity> AvailableTicketsChanged { get; set; }
       public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
    }
}