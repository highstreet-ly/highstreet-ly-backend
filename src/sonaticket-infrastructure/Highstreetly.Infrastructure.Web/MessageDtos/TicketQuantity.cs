using System;

namespace Highstreetly.Infrastructure.MessageDtos
{
    public struct TicketQuantity
    {
        public TicketQuantity(Guid ticketType, int quantity, OrderTicketDetails ticketDetails)
        {
            TicketType = ticketType;
            Quantity = quantity;
            TicketDetails = ticketDetails;
        }

        public Guid TicketType { get; private set; }
        public int Quantity { get; private set; }
        public OrderTicketDetails TicketDetails { get; private set; }

        public string ComparerString => $"{this.TicketType}-{this.TicketDetails?.ExtrasComparer}";
    }
}