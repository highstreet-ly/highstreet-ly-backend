using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IUnassignTicket: ICommand
    {
         Guid TicketAssignmentsId { get; set; }
         int Position { get; set; }
    }
}