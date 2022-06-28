using System;

namespace Highstreetly.Infrastructure.MessageDtos
{
    public class TicketOrderLine : OrderLine
    {
        public Guid TicketType { get; set; }

        public long UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}