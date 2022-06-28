using System.Collections.Generic;

namespace Highstreetly.Infrastructure
{
    /// <summary>
    /// Defines that the object exposes events that are meant to be published.
    /// </summary>
    public interface IEventPublisher
    {
        Dictionary<System.Type, ISonaticketEvent> Events { get; }
    }
}