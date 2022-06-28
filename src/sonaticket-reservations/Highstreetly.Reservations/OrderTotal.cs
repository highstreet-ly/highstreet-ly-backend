using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Reservations
{
    public class OrderTotal
    {
        public ICollection<TicketOrderLine> Lines { get; set; } = new List<TicketOrderLine>();
        public long Total { get; set; }
        public long PaymentPlatformFees { get; set; }
        public long PlatformFees { get; set; }
        public long DeliveryFee { get; set; }
    }
}