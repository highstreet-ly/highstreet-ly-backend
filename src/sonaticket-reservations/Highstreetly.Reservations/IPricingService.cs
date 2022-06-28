using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Reservations
{
    public interface IPricingService
    {
        Task<OrderTotal> CalculateTotal(Guid serviceId, Guid orderId, List<TicketQuantity> seatItems);
    }
}