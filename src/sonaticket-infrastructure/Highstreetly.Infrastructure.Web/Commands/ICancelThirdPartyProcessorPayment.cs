using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface ICancelThirdPartyProcessorPayment : ICommand
    {
        Guid PaymentId { get; set; }
		string Reason { get; set; }
		string Code { get; set;  }
    }
}