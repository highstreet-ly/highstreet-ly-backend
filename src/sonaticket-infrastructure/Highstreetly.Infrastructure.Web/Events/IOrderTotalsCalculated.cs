using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public interface IOrderTotalsCalculated : ISonaticketEvent
    {
        long Total { get; set; }
        TicketOrderLine[] Lines { get; set; }
        bool IsFreeOfCharge { get; set; }

        Guid EventInstanceId { get; set; }

        IEnumerable<TicketQuantity> Tickets { get; set; }

        /// <summary>
        /// The expected expiration time if the reservation is not explicitly confirmed later.
        /// </summary>
        DateTime ReservationAutoExpiration { get; set; }

        string AccessCode { get; set; }
        Guid? OwnerId { get; set; }

        long PaymentPlatformFees { get; set; }
        long PlatformFees { get; set; }
        long DeliveryFee { get; set; }
    }
}