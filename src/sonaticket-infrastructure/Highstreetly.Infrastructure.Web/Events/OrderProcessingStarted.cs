using System;

namespace Highstreetly.Infrastructure.Events
{
    public class OrderProcessingStarted : IOrderProcessingStarted
    {
        public Guid CorrelationId { get; }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public int Version { get; set; }
        public Guid OrderId { get; set; }
    }
}