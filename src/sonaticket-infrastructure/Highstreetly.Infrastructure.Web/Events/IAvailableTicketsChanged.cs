using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public interface IAvailableTicketsChanged : ISonaticketEvent
    {
         IEnumerable<TicketQuantity> Tickets { get; set; }
    }
}
