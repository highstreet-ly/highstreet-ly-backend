using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface IStripeEvent : ISonaticketEvent
    {
        public string HsEventId { get; set; }
    }
}