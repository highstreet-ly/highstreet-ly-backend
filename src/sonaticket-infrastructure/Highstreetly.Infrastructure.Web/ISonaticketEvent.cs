using System;
using MassTransit;

namespace Highstreetly.Infrastructure
{
    //todo: rename
    public interface ISonaticketEvent : CorrelatedBy<Guid>
    {
        /// <summary>
        /// Gets the identifier of the source originating the event.
        /// </summary>
        Guid SourceId { get; set; }

        TimeSpan Delay { get; set; }

        int Version { get; set; }
    }
}