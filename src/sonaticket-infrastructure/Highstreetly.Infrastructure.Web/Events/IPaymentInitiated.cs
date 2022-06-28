using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface IPaymentInitiated : ISonaticketEvent
    {
        Guid PaymentSourceId { get; set; }
    }
}