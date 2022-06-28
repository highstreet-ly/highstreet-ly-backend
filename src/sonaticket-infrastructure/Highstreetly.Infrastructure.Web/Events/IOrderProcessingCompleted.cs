using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface IOrderProcessingCompleted: ISonaticketEvent
    {
        Guid OrderId { get; set; }
    }
}