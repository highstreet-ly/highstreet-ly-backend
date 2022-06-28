using System;

namespace Highstreetly.Infrastructure.Events
{
    public class PaymentCompleted : IPaymentCompleted
    {
        public Guid PaymentSourceId { get; set; }
        public string Email { get; set; }
        public Guid UserId { get; set; }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public int Version { get; set; }
       public Guid CorrelationId { get; set; }
    }
}