using System;

namespace Highstreetly.Infrastructure.Events
{
    public class DeliveryMethodChanged : IDeliveryMethodChanged
    {
        public Guid CorrelationId { get; set; }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public int Version { get; set; }
    }
}