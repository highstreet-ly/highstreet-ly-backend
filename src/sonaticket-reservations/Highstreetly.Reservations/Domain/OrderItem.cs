using System;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Reservations.Domain
{
    public class OrderItem
    {
        public OrderItem(Guid ticketType, int quantity, OrderTicketDetails ticket)
        {
            TicketType = ticketType;
            Quantity = quantity;
            Ticket = ticket;
        }

        public Guid TicketType { get; }
        public int Quantity { get; }
        public OrderTicketDetails Ticket { get; }
    }
}