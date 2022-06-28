using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public interface IOrderPlaced : ISonaticketEvent
    {
        Guid EventInstanceId { get; set; }

        IEnumerable<TicketQuantity> Tickets { get; set; }

        /// <summary>
        /// The expected expiration time if the reservation is not explicitly confirmed later.
        /// </summary>
        DateTime ReservationAutoExpiration { get; set; }

        string AccessCode { get; set; }
        Guid? OwnerId { get; set; }
        string HumanReadableId { get; set; }

        bool IsClickAndCollect { get; set; }
        bool IsLocalDelivery { get; set; }
        bool IsNationalDelivery { get; set; }
        bool IsToTable { get; set; }
        string TableInfo { get; set; }
    }
}
