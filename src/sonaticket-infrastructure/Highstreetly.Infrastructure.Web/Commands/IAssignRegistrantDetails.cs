using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IAssignRegistrantDetails : ICommand
    {
        Guid OrderId { get; set; }
        string OwnerName { get; set; }
        string Email { get; set; }
        Guid UserId { get; set; }
        string Phone { get; set; }
        string DeliveryLine1 { get; set; }
        string DeliveryPostcode { get; set; }
    }
}