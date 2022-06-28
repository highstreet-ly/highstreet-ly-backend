using System;

namespace Highstreetly.Infrastructure.Events
{
    public class PaymentInitiated : IPaymentInitiated
    {
        public Guid PaymentSourceId { get; set; }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public int Version { get; set; }
       public Guid CorrelationId { get; set; }
    }
}
