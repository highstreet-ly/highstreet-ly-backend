using System;
using Highstreetly.Infrastructure.ChargeBee.AddOnUpdated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class UpdateAddOn : IUpdateAddOn
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public AddOnUpdate AddOnUpdate { get; set; }
    }
}