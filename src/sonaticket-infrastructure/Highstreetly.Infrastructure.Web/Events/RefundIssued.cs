using System;

namespace Highstreetly.Infrastructure.Events
{
    public class RefundIssued : IRefundIssued
    {
        public Guid CorrelationId { get; set;  }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public int Version { get; set; }
        public int Amount { get; set; }
    }
}