using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class IssueRefund : IIssueRefund
    {
        public Guid CorrelationId { get; }
        public Guid Id { get; }
        public Guid RefundId { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public Guid ChargeId { get; set; }
    }
}