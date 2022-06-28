using System;

namespace Highstreetly.Infrastructure.Events
{
    public class SetDeliveryMethod : ISetDeliveryMethod
    {
        public Guid CorrelationId { get; set; }
        public Guid SourceId { get; set; }
        public Guid EventInstanceId { get; set; }
        public Guid Id { get; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public bool IsClickAndCollect { get; set; }
        public bool IsLocalDelivery { get; set; }
        public bool IsNationalDelivery { get; set; }
        public bool IsToTable { get; set; }
        public string TableInfo { get; set; }
    }
}