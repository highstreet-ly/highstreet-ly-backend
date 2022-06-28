using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Commands
{
    public class MarkTicketsAsReserved : IMarkTicketsAsReserved
    {
        public MarkTicketsAsReserved()
        {
            Id = Guid.NewGuid();
            Tickets = new List<TicketQuantity>();
        }

        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
       public Guid CorrelationId { get; set; }

        public Guid OrderId { get; set; }

        public List<TicketQuantity> Tickets { get; set; }

        public DateTime Expiration { get; set; }
    }
}