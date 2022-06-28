using System;

namespace Highstreetly.Infrastructure.Events
{
    public interface ISetDeliveryMethod : ICommand
    {
        Guid SourceId { get; set; }
        Guid EventInstanceId { get; set; }

        bool IsClickAndCollect { get; set; }
        bool IsLocalDelivery { get; set; }
        bool IsNationalDelivery { get; set; }
        bool IsToTable { get; set; }
        string TableInfo { get; set; }
    }
}