using System;
using MassTransit;

namespace Highstreetly.Payments.Domain
{
    public class ThirdPartyProcessorPaymentItem
    {
        public ThirdPartyProcessorPaymentItem(string description, long amount)
        {
            Id = NewId.NextGuid();

            Description = description;
            Amount = amount;
        }

        protected ThirdPartyProcessorPaymentItem()
        {
        }

        public Guid Id { get; private set; }

        public string Description { get; private set; }

        public long Amount { get; private set; }
    }
}