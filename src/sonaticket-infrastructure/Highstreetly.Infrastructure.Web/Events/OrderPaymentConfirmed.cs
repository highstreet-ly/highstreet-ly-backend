using System;

namespace Highstreetly.Infrastructure.Events
{
    public class OrderPaymentConfirmed : IOrderPaymentConfirmed
    {
        public Guid SourceId { get; set;  }
        public TimeSpan Delay { get; set; }
        public Guid CorrelationId { get; set; }
        public int Version { get; set;  }
    }
}