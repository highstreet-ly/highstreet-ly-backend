using System;

namespace Highstreetly.Infrastructure.Events
{
    public class DraftOrderCreated : IDraftOrderCreated
    {
        public Guid OrderId { get; set; }
        public Guid Id { get; }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public int Version { get; set; }
        public string TypeInfo { get; set; }
        public Guid EventInstanceId { get; set; }
        public Guid CorrelationId { get; set; }
        public string HumanReadableId { get; set; }
        public bool IsClickAndCollect { get; set; }
        public bool IsLocalDelivery { get; set; }
        public bool IsNationalDelivery { get; set; }
        public bool IsToTable { get; set; }
        public string TableInfo { get; set; }
    }
}