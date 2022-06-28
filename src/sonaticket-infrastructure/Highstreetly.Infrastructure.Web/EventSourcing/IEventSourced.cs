using System;
using System.Collections.Generic;
using MassTransit;

namespace Highstreetly.Infrastructure.EventSourcing
{
    /// <summary>
    /// Represents an identifiable entity that is event sourced.
    /// </summary>
    public interface IEventSourced : CorrelatedBy<Guid>
    {
        /// <summary>
        /// Gets the entity identifier.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Gets the entity's version. As the entity is being updated and events being generated, the version is incremented.
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// Gets the collection of new events since the entity was loaded, as a consequence of command handling.
        /// </summary>
        Dictionary<Type, ISonaticketEvent> Events { get; }
        void ClearUncommittedEvents();
    }
}