using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface IDraftOrderCreated : ISonaticketEvent
    {
        Guid OrderId { get; set; }
        Guid EventInstanceId { get; set; }
        string HumanReadableId { get; set; }

        public bool IsClickAndCollect { get; set; }
        public bool IsLocalDelivery { get; set; }
        public bool IsNationalDelivery { get; set; }
        public bool IsToTable { get; set; }
        public string TableInfo { get; set; }
    }
}