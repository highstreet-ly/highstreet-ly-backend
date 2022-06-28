using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class CommitTicketReservation : ICommitTicketReservation
    {
        public Guid ReservationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
       public Guid CorrelationId { get; set; }
        public Guid EventInstanceId { get; set; }
        public string SessionId { get; }
    }
}