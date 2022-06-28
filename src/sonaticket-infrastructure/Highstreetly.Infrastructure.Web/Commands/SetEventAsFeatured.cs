using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class SetEventAsFeatured : ISetEventAsFeatured
    {
        public Guid CorrelationId { get; set;  }
        public Guid Id { get; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public Guid SourceId { get; set; }
        public bool Published { get; set; }
        public string Slug { get; set; }
    }
}