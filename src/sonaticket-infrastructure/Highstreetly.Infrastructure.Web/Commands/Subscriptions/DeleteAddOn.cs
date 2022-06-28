using System;
using Highstreetly.Infrastructure.ChargeBee.AddOnDeleted;

namespace Highstreetly.Infrastructure.Commands.Subscriptions
{
    public class DeleteAddOn : IDeleteAddOn
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public AddOnDelete AddOnDelete { get; set; }
    }
}