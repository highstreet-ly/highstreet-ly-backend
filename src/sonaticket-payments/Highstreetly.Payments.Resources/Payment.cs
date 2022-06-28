using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Payments.Resources
{
    [Resource("payments")]
    [Table("Payments", Schema = "TicketedEventPayments")]
    public class Payment : Identifiable<Guid>
    {
        public Payment()
        {
            Charges = new List<Charge>();
        }

        [Attr]
        public string PaymentIntentId { get; set; }

        [Attr]
        public string PaymentIntentSecret { get; set; }

        [Attr]
        public string Token { get; set; }

        [Attr]
        public string Email { get; set; }

        [Attr]
        public Guid EventInstanceId { get; set; }

        [Attr]
        public Guid OrderId { get; set; }

        [Attr]
        public int OrderVersion { get; set; }

        [Attr]
        [NotMapped]
        public Guid PaymentId { get => Id; set => Id = value; }

        [HasMany] 
        public List<Charge> Charges { get; set; }
        
        [Attr] public DateTime DateCreated { get; set; }
    }
}
