using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public interface ICommitOrder : ICommand
    {
        Guid SourceId { get; set; }
        List<TicketQuantity> Tickets { get; set; }
    }
}