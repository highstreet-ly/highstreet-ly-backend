using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface IDraftOrderUpdated : ISonaticketEvent
    {
        Guid OrderId { get; set; }
    }
}