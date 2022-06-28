using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.MessageDtos;
using Newtonsoft.Json;

namespace Highstreetly.Infrastructure.Commands
{
    public class AddTicketsToBasket : IAddTicketsToBasket
    {
        public AddTicketsToBasket()
        {
            Id = Guid.NewGuid();
            Tickets = new List<TicketQuantity>();
        }

        public Guid Id { get; set; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }

        public Guid OrderId { get; set; }

        public Guid EventInstanceId { get; set; }

        public string Slug { get; set; }

        public string EventInstanceName { get; set; }

        [JsonProperty("tickets")]
        public IList<TicketQuantity> Tickets { get; set; }

        public int OrderVersion { get; set; }
        public Guid? OwnerId { get; set; }
        public string OwnerEmail { get; set; }
        public Guid CorrelationId { get; set; }
        public string HumanReadableId { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Tickets == null || !Tickets.Any(x => x.Quantity > 0))
        //    {
        //        return new[] { new ValidationResult("One or more items are required.", new[] { "Tickets" }) };
        //    }
        //    else if (Tickets.Any(x => x.Quantity < 0))
        //    {
        //        return new[] { new ValidationResult("Invalid registration.", new[] { "Tickets" }) };
        //    }

        //    return Enumerable.Empty<ValidationResult>();
        //}
    }
}
