using System;

namespace Highstreetly.Management.Resources
{
    public interface ITicketType
    {
        Guid Id { get; set; }
        Guid EventInstanceId { get; set; }
        string Name { get; set; }
        string EventSlug { get; set; }
        string Description { get; set; }
        long? Price { get; set; }
        int? Quantity { get; set; }
        int? AvailableQuantity { get; set; }
        int TicketsAvailabilityVersion { get; set; }
    }
}