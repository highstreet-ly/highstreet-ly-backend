using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Payments.Resources
{
    [Table("ThirdPartyPayment", Schema = "TicketedEventPayments")]
    [Resource("third-party-payments")]
    public class ThirdPartyPayment : Identifiable<Guid>
    {
        [Attr]
        public PaymentStates State { get; set; }
        
        [Attr]
        public Guid PaymentSourceId { get; set; }
        
        [Attr]
        public string Description { get; set; }
        
        [Attr]
        public long? TotalAmount { get; set; }
        
        [Attr]
        public long? Amount { get; set; }
        
        [Attr]
        public long? ApplicationFeeAmount { get; set; }
        
        [Attr]
        public string Last4 { get; set; }
        
        [Attr]
        public DateTime Created { get; set; }
        
        [Attr]
        public string OutcomeDescription { get; set; }
        
        [Attr]
        public string OutcomeCode { get; set; }
        
        [Attr]
        public string Currency { get; set; }
    }
}