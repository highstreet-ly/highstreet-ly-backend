using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IAddTicketsToBasket : ICommand
    {
        Guid OrderId { get; set; }
        Guid EventInstanceId { get; set; }
        string Slug { get; set; }
        string EventInstanceName { get; set; }
        IList<TicketQuantity> Tickets { get; set; }
        int OrderVersion { get; set; }
        Guid? OwnerId { get; set; }
        string OwnerEmail { get; set; }

        string HumanReadableId { get; set; }
        //IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}
