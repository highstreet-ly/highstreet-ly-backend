using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IIssueRefund : ICommand
    {
        Guid ChargeId { get; set; }
        Guid RefundId { get; set; }
    }
}