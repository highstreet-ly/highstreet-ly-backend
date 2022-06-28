using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("SubscriptionAddOns", Schema = "Management")]
    public class SubscriptionAddOn
    {
        public Guid SubscriptionId { get; set; }

        [HasOne]
        public Subscription Subscription { get; set; }

        public Guid AddOnId { get; set; }

        [HasOne]
        public AddOn AddOn { get; set; }
    }
}