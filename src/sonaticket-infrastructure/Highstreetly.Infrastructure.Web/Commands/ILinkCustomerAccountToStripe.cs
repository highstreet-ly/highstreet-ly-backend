using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface ILinkCustomerAccountToStripe : ICommand
    {
        Guid EventOrganiserId { get; set; }
        Guid CustomerId { get; set; }

        string PaymentIntentId { get; set; }
    }
}