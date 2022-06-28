using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Reservations.Resources
{
    [Resource("product-extras")]
    [Table("ProductExtra", Schema = "Reservation")]
    public class ProductExtra : Identifiable<Guid>
    {
        [Attr] 
        public override Guid Id { get; set; }
        
        /// <summary>
        /// We need this to reference the PE from the mgmt BC
        /// Otherwise we have nothing to tie it back to the original config
        /// </summary>
        [Attr] 
        public Guid ReferenceProductExtraId { get; set; }
        
        [Attr]
        public Guid? PricedOrderLineId { get; set; }

        [HasOne]
        public PricedOrderLine PricedOrderLine { get; set; }
        
        [Attr]
        public Guid? OrderTicketDetailsId { get; set; }

        [HasOne]
        public OrderTicketDetails OrderTicketDetails { get; set; }

        [Attr]
        public string Name { get; set; }

        [Attr]
        public string Description { get; set; }

        [Attr]
        public long Price { get; set; }

        [Attr]
        public bool Selected { get; set; }

        [Attr]
        public long ItemCount { get; set; } = 0;
    }
}