using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Payments.Resources
{
    /// <summary>
    /// TODO: add the rest of these: https://stripe.com/docs/api/charges/object
    /// </summary>
    [Resource("charges")]
    [Table("Charges", Schema = "TicketedEventPayments")]
    public class Charge : Identifiable<Guid>
    {
        public Charge()
        {
            Refunds = new List<Refund>();
        }

        [Attr] public string ChargeId { get; set; }

        [Attr] public string PaymentIntent { get; set; }

        [Attr] public bool Refunded { get; set; }

        [HasMany] public List<Refund> Refunds { get; set; }

        public Guid PaymentId { get; set; }

        [HasOne] public Payment Payment { get; set; }

        [Attr] public long Amount { get; set; }

        [Attr] public long AmountCaptured { get; set; }

        [Attr] public long AmountRefunded { get; set; }

        [Attr] public long ApplicationFee { get; set; }

        [Attr] public long ApplicationFeeAmount { get; set; }

        [Attr] public string Application { get; set; }

        [Attr] public string Currency { get; set; }

        [Attr] public string Description { get; set; }

        [Attr] public string FailureCode { get; set; }

        [Attr] public string FailureMessage { get; set; }

        [Attr] public string RecieptUrl { get; set; }
        
        [Attr] public DateTime DateCreated { get; set; }
    }
}