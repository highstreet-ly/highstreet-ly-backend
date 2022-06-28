using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public class OrderTotalsCalculated : IOrderTotalsCalculated
    {
        public long Total { get; set; }

        public TicketOrderLine[] Lines { get; set; }

        public bool IsFreeOfCharge { get; set; }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public Guid CorrelationId { get; set; }
        public int Version { get; set;  }

        public Guid EventInstanceId { get; set; }

        public IEnumerable<TicketQuantity> Tickets { get; set; }

        /// <summary>
        /// The expected expiration time if the reservation is not explicitly confirmed later.
        /// </summary>
        public DateTime ReservationAutoExpiration { get; set; }

        public string AccessCode { get; set; }
        public Guid? OwnerId { get; set; }
        public long PaymentPlatformFees { get; set; }
        public long PlatformFees { get; set; }
        public long DeliveryFee { get; set; }
    }
}