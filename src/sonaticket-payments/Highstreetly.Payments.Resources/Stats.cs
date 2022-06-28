using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Payments.Resources
{
    [Resource("stats")]
    [Table("stats_by_event_instance", Schema = "public")]
    public class Stats : Identifiable<Guid>
    {
        [Attr] public long? Refunded { get; set; }

        [Attr] public long? Charged { get; set; }

        [Attr] public Guid EventInstanceId { get; set; }
    }
}