using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Reservations.Resources
{
    [Resource("draft-order-items")]
    [Table("DraftOrderItem", Schema = "Reservation")]
    public class DraftOrderItem : Identifiable<Guid>
    {
        [Attr]
        public Guid DraftOrderId { get; set; }
        
        [HasOne] 
        public DraftOrder DraftOrder { get; set; }

        [Attr]
        public Guid TicketType { get; set; }

        [Attr]
        public int RequestedTickets { get; set; }

        [HasOne]
        public OrderTicketDetails Ticket { get; set; }

        [Attr]
        public int ReservedTickets { get; set; }
    }
}