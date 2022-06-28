using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Reservations.Resources
{
    [Resource("priced-order-line")]
    [Table("PricedOrderLine", Schema = "Reservation")]
    public class PricedOrderLine : Identifiable<Guid>
    {
        [Attr]
        public int Position { get; set; }

        [Attr]
        public string Description { get; set; }

        [Attr]
        public long UnitPrice { get; set; }

        [Attr] 
        public int Quantity { get; set; }

        [Attr] 
        public long LineTotal { get; set; }

        [HasMany]
        public List<ProductExtra> ProductExtras { get; set; }

        [Attr] 
        public string Name { get; set; }

        [Attr] 
        public Guid PricedOrderId { get; set; }

        [HasOne] 
        public PricedOrder PricedOrder { get; set; }

        [Attr]
        public Guid TicketType { get; set; }

    }
}