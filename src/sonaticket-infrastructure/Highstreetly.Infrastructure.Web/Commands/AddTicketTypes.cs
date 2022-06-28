using System;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Commands
{
    public class AddTicketTypes : IAddTicketTypes
    {
        public Guid CorrelationId { get; set;  }
        public Guid Id { get; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public string SessionId { get; }
        public Guid EventInstanceId { get; set; }
        public Guid TicketType { get; set; }
        public int Quantity { get; set; }
        public OrderTicketDetails TicketDetails { get; set; }
    }
}