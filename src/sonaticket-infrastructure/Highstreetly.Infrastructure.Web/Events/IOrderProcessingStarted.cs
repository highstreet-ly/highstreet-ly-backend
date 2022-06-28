using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface IOrderProcessingStarted: ISonaticketEvent
    {
        Guid OrderId { get; set; }
    }
}