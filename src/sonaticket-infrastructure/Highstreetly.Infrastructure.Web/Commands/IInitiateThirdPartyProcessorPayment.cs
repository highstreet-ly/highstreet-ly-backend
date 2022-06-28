using System;
using System.Collections.Generic;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IInitiateThirdPartyProcessorPayment : ICommand
    {
        Guid PaymentId { get; set; }
        Guid PaymentSourceId { get; set; }
        Guid EventInstanceId { get; set; }
        string Description { get; set; }
        long TotalAmount { get; set; }
        IList<InitiateThirdPartyProcessorPayment.PaymentItem> Items { get; set; }
        string PaymentIntentId { get; set; }
    }
}
