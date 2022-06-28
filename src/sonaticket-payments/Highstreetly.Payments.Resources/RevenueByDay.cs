using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Payments.Resources
{
    [Resource("revenue-by-day")]
    [Table("amount_captured_by_day_event_instance", Schema = "public")]
    public class RevenueByDay : Identifiable<Guid>
    {
        [Attr] public long? AmountCaptured { get; set; }
        
        [Attr] public double Day { get; set; }
        
        [Attr] public double Year { get; set; }
        
        [Attr] public double Month { get; set; }

        [Attr] public DateTime DateCaptured { get; set; }

        [Attr] public Guid? EventInstanceId { get; set; }
    }
}