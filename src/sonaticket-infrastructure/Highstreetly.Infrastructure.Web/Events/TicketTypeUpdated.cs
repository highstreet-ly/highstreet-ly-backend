using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public class TicketTypeUpdated : ITicketTypeUpdated
    {
        public Guid CorrelationId { get; set;  }
        
        public Guid SourceId { get; set; }
        
        public TimeSpan Delay { get; set; }
        
        public int Version { get; set; }
        
        public Guid EventInstanceId { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public long? Price { get; set; }
        
        public bool FreeTier { get; set; }
        
        public int Quantity { get; set; }
        
        public string MainImageId { get; set; }
        
        public string Tags { get; set; }

        public string CommandSource { get; set; }

        public bool IsPublished { get; set; }
        
        public List<ProductExtraGroup> ProductExtraGroups { get; set; }
        
        public string Group { get; set; }

        public bool UpdatePrice { get; set; }
        
    }
}