using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public class AvailableTicketsChanged : IAvailableTicketsChanged
    {
        public IEnumerable<TicketQuantity> Tickets { get; set; }
        public Guid SourceId { get; set;  }
        public TimeSpan Delay { get; set; }
        public Guid CorrelationId { get; set;  }
        public int Version { get; set;  }
    }
}