using System;

namespace Highstreetly.Infrastructure.Events
{
    public class TicketReservationCommitted : ITicketReservationCommitted
    {
        public Guid ReservationId { get; set; }
       public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
    }
}