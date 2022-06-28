using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface IOrderRegistrantAssigned : ISonaticketEvent
    {
         string OwnerName { get; set; }
         string Email { get; set; }
         Guid UserId { get; set; }
         string Phone { get; set; }
         string DeliveryLine1 { get; set; }
         string DeliveryPostcode { get; set; }
    }
}