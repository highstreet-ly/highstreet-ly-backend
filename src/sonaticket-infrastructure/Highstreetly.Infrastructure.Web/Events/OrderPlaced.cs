using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public class OrderPlaced : IOrderPlaced
    {
        public Guid EventInstanceId { get; set; }

        public IEnumerable<TicketQuantity> Tickets { get; set; }

        /// <summary>
        /// The expected expiration time if the reservation is not explicitly confirmed later.
        /// </summary>
        public DateTime ReservationAutoExpiration { get; set; }

        public string AccessCode { get; set; }
        public Guid? OwnerId { get; set; }
        public string OwnerEmail { get; set; }
        public Guid SourceId { get; set;  }
        public TimeSpan Delay { get; set; }
        public Guid CorrelationId { get; set; }
        public int Version { get; set;  }
        public string HumanReadableId { get; set; }
        public bool IsClickAndCollect { get; set; }
        public bool IsLocalDelivery { get; set; }
        public bool IsNationalDelivery { get; set; }
        public bool IsToTable { get; set; }
        public string TableInfo { get; set; }
    }
}
