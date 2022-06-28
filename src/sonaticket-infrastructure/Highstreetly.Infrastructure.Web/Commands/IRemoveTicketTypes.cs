using System;
using Highstreetly.Infrastructure.Messaging;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IRemoveTicketTypes : ITicketTypeAvailabilityCommand
    {
        Guid TicketType { get; set; }
        int Quantity { get; set; }
    }
}