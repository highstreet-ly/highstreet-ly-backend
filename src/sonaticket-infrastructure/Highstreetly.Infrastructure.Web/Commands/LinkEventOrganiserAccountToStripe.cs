using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class LinkEventOrganiserAccountToStripe : ILinkEventOrganiserAccountToStripe
    {
        public Guid CorrelationId { get; set;  }
        public Guid Id { get; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public string Code { get; set; }
        public Guid EventOrganiserId { get; set; }
        public string Scope { get; set; }
    }
}