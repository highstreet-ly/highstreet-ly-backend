using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface ITicketTypeUnpublished : ISonaticketEvent
    {
        Guid EventInstanceId { get; set; }
        string Name { get; set; }
        string MainImageId { get; set; }
        string Tags { get; set; }
    }
}