using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Reservations.Resources
{
    [Table("OrderTicketDetails", Schema = "Reservation")]
    [Resource("ticket-details")]
    public class OrderTicketDetails: Identifiable<Guid>
    {
        public OrderTicketDetails()
        {
            ProductExtras = new List<ProductExtra>();
        }

        [Attr]
        public override Guid Id { get; set; }
        
        [Attr]
        public Guid? DraftOrderItemId { get; set; }

        [HasOne]
        public DraftOrderItem DraftOrderItem { get; set; }

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

        [NotMapped]
        public string ExtrasComparer
        {
            get
            {
                return $"{this.Id}-{string.Join(",", ProductExtras.Select(x => x.Name))}";
            }
        }
    }
}