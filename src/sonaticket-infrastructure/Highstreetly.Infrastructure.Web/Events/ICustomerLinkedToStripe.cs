using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface ICustomerLinkedToStripe
    {
        string StripeCustomerId { get; set; }
        string PaymentIntentId { get; set; }
        Guid EventOrganiserId { get; set; }
    }
}