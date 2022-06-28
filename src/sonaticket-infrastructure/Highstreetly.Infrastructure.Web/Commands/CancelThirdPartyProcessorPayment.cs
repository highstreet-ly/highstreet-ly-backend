using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class CancelThirdPartyProcessorPayment : ICancelThirdPartyProcessorPayment
    {
        public CancelThirdPartyProcessorPayment()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid PaymentId { get; set; }
        public string Reason { get; set; }
        public string Code { get; set; }
    }
}