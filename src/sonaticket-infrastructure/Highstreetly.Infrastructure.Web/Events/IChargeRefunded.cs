using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface IChargeRefunded: IStripeEvent
    {
        Guid OrderId { get; set; }
        string ReceiptUrl { get; set; }
    }
}