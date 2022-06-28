using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Payments.Resources
{
    [Resource("HsStripeEvents")]
    [Table("HsStripeEvent", Schema = "TicketedEventPayments")]
    public class HsStripeEvent
    {
        public Guid Id { get; set; }
        [Column(TypeName = "jsonb")] 
        public string Data { get; set; }
    }
}