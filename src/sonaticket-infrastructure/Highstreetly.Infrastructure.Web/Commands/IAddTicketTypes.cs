using System;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Infrastructure.Messaging;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IAddTicketTypes : ITicketTypeAvailabilityCommand
    {
        Guid TicketType { get; set; }
        int Quantity { get; set; }
        OrderTicketDetails TicketDetails { get; set; }
    }
}