using System;

namespace Highstreetly.Infrastructure.Events
{
    public class CustomerLinkedToStripe : ICustomerLinkedToStripe
    {
        public string StripeCustomerId { get; set; }
        public string PaymentIntentId { get; set; }
        public Guid EventOrganiserId { get; set; }
    }
}