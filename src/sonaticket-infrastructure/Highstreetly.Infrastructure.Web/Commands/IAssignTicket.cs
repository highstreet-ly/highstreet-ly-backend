using System;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IAssignTicket : ICommand
    {
        Guid TicketAssignmentsId { get; set; }
        int Position { get; set; }
        PersonalInfo Attendee { get; set; }
    }
}