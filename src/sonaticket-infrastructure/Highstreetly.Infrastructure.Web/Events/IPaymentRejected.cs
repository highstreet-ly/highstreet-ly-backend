using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface IPaymentRejected : ISonaticketEvent
    {
        Guid PaymentSourceId { get; set; }
    }
}