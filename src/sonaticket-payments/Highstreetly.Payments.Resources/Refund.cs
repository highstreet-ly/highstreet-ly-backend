using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Payments.Resources
{
    [Resource("refunds")]
    [Table("Refunds", Schema = "TicketedEventPayments")]
    public class Refund : Identifiable<Guid>
    {
        public Refund()
        {
            
        }
        
        public Guid ChargeId { get; set; }
        
        [HasOne]
        public Charge Charge { get; set; }
        
        [Attr] 
        public string Reason { get; set; }
        
        [Attr]
        public string Currency { get; set; }
        
        [Attr]
        public string PaymentIntent { get; set; }
        
        [Attr] 
        public string ReceiptNumber { get; set; }
        
        [Attr] 
        public bool? SourceTransferReversal { get; set; }
        
        [Attr] 
        public bool? TransferReversal { get; set; }
        
        [Attr] 
        public string Status { get; set; }
        
        [Attr] 
        public int Amount { get; set; }
        
        [Attr] 
        public string RefundNote { get; set; }

        [Attr] public DateTime DateCreated { get; set; }
    }
}