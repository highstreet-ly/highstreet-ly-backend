using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class CompleteThirdPartyProcessorPayment : ICompleteThirdPartyProcessorPayment
    {
        public CompleteThirdPartyProcessorPayment()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public Guid PaymentId { get; set; }
        public long? Amount { get; set; }
        public long? ApplicationFeeAmount { get; set; }
        public string Last4 { get; set; }
        public DateTime Created { get; set; }
        public string Currency { get; set; }
        public string Email { get; set; }
        public Guid UserId { get; set; }
        public string PaymentIntentId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid OrderId { get; set; }
    }
}