using System;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public class ChargeRefunded : StripeEvent, IChargeRefunded
    {
        public Guid OrderId { get; set; }
        public string ReceiptUrl { get; set; }
    }
}