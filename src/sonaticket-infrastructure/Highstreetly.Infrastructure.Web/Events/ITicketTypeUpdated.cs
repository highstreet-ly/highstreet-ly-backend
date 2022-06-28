using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public interface ITicketTypeUpdated : ISonaticketEvent
    {
        Guid EventInstanceId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        long? Price { get; set; }
        bool FreeTier { get; set; }
        int Quantity { get; set; }

        string MainImageId { get; set; }
        string Tags { get; set; }

        string CommandSource { get; set; }

        bool IsPublished { get; set; }

        List<ProductExtraGroup> ProductExtraGroups { get; set; }
        string Group { get; set; }
        bool UpdatePrice { get; set; }
    }
}