using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class AssignRegistrantDetails : IAssignRegistrantDetails
    {
        public AssignRegistrantDetails()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        
        public string TypeInfo { get; set; }
        public Guid CorrelationId { get; set; }

        public Guid OrderId { get; set; }


        public string OwnerName { get; set; }

        public string Email { get; set; }
        public Guid UserId { get; set; }
        public string Phone { get; set; }
        public string DeliveryLine1 { get; set; }
        public string DeliveryPostcode { get; set; }
    }
}