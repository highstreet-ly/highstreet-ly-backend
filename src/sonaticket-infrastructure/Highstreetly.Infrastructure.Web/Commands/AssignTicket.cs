using System;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Commands
{
    public class AssignTicket : IAssignTicket
    {
        public AssignTicket()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid TicketAssignmentsId { get; set; }
        public int Position { get; set; }
        public PersonalInfo Attendee { get; set; }
    }
}