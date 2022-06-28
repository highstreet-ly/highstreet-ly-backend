using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class RemoveTicketTypes : IRemoveTicketTypes
    {
        public Guid CorrelationId { get; set; }
        public Guid Id { get; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public string SessionId { get; }
        public Guid EventInstanceId { get; set; }
        public Guid TicketType { get; set; }
        public int Quantity { get; set; }
    }
}