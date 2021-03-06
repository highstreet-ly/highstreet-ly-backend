using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface ITicketTypePublished : ISonaticketEvent
    {
        Guid EventInstanceId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        long? Price { get; set; }
        int Quantity { get; set; }
        string MainImageId { get; set; }
        string Tags { get; set; }

    }
}