using System;

namespace Highstreetly.Infrastructure.Events
{
    public class OrderRegistrantAssigned : IOrderRegistrantAssigned
    {
        public string OwnerName { get; set; }
        public string Email { get; set; }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public Guid UserId { get; set; }
        public string Phone { get; set; }
        public string DeliveryLine1 { get; set; }
        public string DeliveryPostcode { get; set; }
    }
}