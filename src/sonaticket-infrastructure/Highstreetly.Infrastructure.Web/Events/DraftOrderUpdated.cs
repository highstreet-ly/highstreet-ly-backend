using System;

namespace Highstreetly.Infrastructure.Events
{
    public class DraftOrderUpdated : IDraftOrderUpdated
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid Id { get; }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public int Version { get; set; }
        public string TypeInfo { get; set; }
    }
}