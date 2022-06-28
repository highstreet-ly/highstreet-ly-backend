using System;

namespace Highstreetly.Infrastructure.MessageDtos
{
    public class TicketAssignmentInfo
    {
        public int Position { get; set; }
        public Guid TicketType { get; set; }
    }
}