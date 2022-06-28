using System;
using Highstreetly.Infrastructure.ChargeBee.AddOnCreated;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class CreateAddOn : ICreateAddOn
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public AddOnCreate AddOnCreate { get; set; }
    }
}