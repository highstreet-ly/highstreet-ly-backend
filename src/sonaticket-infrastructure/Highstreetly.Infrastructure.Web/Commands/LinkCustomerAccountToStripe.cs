using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class LinkCustomerAccountToStripe : ILinkCustomerAccountToStripe
    {
        public Guid CorrelationId { get; }
        public Guid Id { get; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public Guid EventOrganiserId { get; set; }
        public Guid CustomerId { get; set; }
        public string PaymentIntentId { get; set; }
    }
}