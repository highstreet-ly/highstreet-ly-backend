using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("OrderTicket", Schema = "Management")]
    [Resource("order-tickets")]
    public class OrderTicket: Identifiable<Guid>
    {
        public OrderTicket()
        {
            // Complex type properties can never be null.
            //Attendee = new Attendee();
        }
        
        [Attr]
        public int Position { get; set; }

        // [Attr]
        // [Column(TypeName = "jsonb")]
        // public Attendee Attendee { get; set; }

        [Attr]
        public Guid TicketTypeConfigurationId { get; set; }

        [HasOne]
        public TicketTypeConfiguration TicketTypeConfiguration { get; set; }

        [HasOne]
        public OrderTicketDetails TicketDetails { get; set; }
        
        [HasOne] 
        public Order Order { get; set; }
        
        [Attr]
        public Guid OrderId { get; set; }
        
        [Attr]
        public bool Refunded { get; set; }
    }
}