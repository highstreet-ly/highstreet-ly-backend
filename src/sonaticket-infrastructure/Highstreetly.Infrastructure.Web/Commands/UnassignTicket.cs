using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class UnassignTicket: IUnassignTicket
    {
        public UnassignTicket()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
       public Guid CorrelationId { get; set; }

        public Guid TicketAssignmentsId { get; set; }
        public int Position { get; set; }
    }
}