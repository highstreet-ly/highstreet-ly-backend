using System;
using System.Collections.Generic;

namespace Highstreetly.Infrastructure.Commands
{
    public class InitiateThirdPartyProcessorPayment : IInitiateThirdPartyProcessorPayment
    {
        public class PaymentItem
        {
            public string Description { get; set; }

            public long Amount { get; set; }
        }

        public InitiateThirdPartyProcessorPayment()
        {
            Id = Guid.NewGuid();
            Items = new List<PaymentItem>();
        }

        public Guid Id { get; private set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }

        public Guid PaymentId { get; set; }

        public Guid PaymentSourceId { get; set; }

        public Guid EventInstanceId { get; set; }

        public string Description { get; set; }

        public long TotalAmount { get; set; }

        public IList<PaymentItem> Items { get;  set; }

        public string PaymentIntentId { get; set; }
        public string PaymentIntentClientSecret { get; set; }

        public Guid CorrelationId { get; set; }
    }
}