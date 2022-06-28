using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class SetOrderProcessingComplete : ISetOrderProcessingComplete
    {
        public Guid CorrelationId { get; }
        public Guid Id { get; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public Guid OrderId { get; set; }
    }
}