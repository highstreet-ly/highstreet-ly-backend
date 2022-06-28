using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IMarkTicketsAsReserved : ICommand
    {
         Guid OrderId { get; set; }
         List<TicketQuantity> Tickets { get; set; }
         DateTime Expiration { get; set; }
    }
}