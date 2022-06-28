using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Payments.ViewModels.Payments
{
    public class StripeCustomer : IIdentifiable<string>
    {
        [Attr] 
        public string Id { get; set; }

        [Attr] 
        public int AccountBalance { get; set; }

        [NotMapped]
        public int Created { get; set; }

        [Attr] 
        public string Currency { get; set; }

        [Attr] 
        public string DefaultSource { get; set; }

        [Attr] 
        public bool Delinquent { get; set; }

        [Attr] 
        public string Description { get; set; }

        [Attr] 
        public string Discount { get; set; }

        [Attr] 
        public string Email { get; set; }

        [Attr] 
        public bool Livemode { get; set; }

        [Attr] 
        public string Shipping { get; set; }

        [Attr] 
        public Guid SonaticketUserId { get; set; }

        [NotMapped] 
        public string StringId { get => Id; set => Id = value; }

        [NotMapped]
        public string LocalId { get; set; }
    }
}
