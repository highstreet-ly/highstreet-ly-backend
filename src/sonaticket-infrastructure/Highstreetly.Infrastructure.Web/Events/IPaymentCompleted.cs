using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface IPaymentCompleted : ISonaticketEvent
    {
        Guid PaymentSourceId { get; set; }
        string Email { get; set; }

        Guid UserId { get; set; }
    }
}