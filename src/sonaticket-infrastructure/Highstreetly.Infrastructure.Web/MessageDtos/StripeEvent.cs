using System;

namespace Highstreetly.Infrastructure.MessageDtos
{
    public abstract class StripeEvent : ISonaticketEvent
    {
        public string HsEventId { get; set; }
        public Guid CorrelationId { get; }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public int Version { get; set; }
    }
}