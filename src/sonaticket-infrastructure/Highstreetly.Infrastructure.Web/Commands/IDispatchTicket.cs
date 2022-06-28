using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IDispatchTicket : ICommand
    {
        Guid TicketAssignmentsId { get; set; }
        int Position { get; set; }
    }
}