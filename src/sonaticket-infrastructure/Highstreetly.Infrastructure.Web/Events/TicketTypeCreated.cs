using System;

namespace Highstreetly.Infrastructure.Events
{
    public class TicketTypeCreated : ITicketTypeCreated
    {
        public Guid CorrelationId { get; set;  }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public int Version { get; set; }
        public Guid EventInstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? Price { get; set; }
        public int Quantity { get; set; }
       
        public string MainImageId { get; set; }
        public string Tags { get; set; }
        public bool IsPublished { get; set; }
    }
}