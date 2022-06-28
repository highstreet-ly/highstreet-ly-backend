using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("OrderTicketDetails", Schema = "Management")]
    [Resource("ticket-details")]
    public class OrderTicketDetails: Identifiable<Guid>
    {
        [Attr]
        public Guid OrderTicketId { get; set; }

        [HasOne]
        public OrderTicket OrderTicket { get; set; }

        [Attr]
        public Guid EventInstanceId { get; set; }

        [Attr]
        public string Name { get; set; }

        [Attr]
        public string DisplayName { get; set; }

        [Attr]
        public long Price { get; set; }

        [Attr]
        public int Quantity { get; set; }

        [HasMany]
        public List<ProductExtra> ProductExtras { get; set; }
    }
}