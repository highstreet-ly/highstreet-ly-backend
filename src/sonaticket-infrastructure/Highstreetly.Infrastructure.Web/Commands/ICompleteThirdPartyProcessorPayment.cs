using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface ICompleteThirdPartyProcessorPayment : ICommand
    {
        Guid PaymentId { get; set; }
        string PaymentIntentId { get; set; }
        long? Amount { get; set; }
        long? ApplicationFeeAmount { get; set; }
        string Last4 { get; set; }
        DateTime Created { get; set; }
        string Currency { get; }
        string Email { get; set; }
        Guid UserId { get; set; }
        Guid OrderId { get; set; }
    }
}