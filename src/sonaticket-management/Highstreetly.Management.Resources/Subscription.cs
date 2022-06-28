using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("Subscription", Schema = "Management")]
    [Resource("subscriptions")]
    public class Subscription : Identifiable<Guid>
    {
        public Subscription()
        {
            AddOns = new List<AddOn>();
        }


        [Attr]
        public string IntegrationId { get; set; }


        [Attr]
        public string CustomerId { get; set; }

        [HasOne]
        public Plan Plan { get; set; }

        [Attr]
        public Guid PlanId { get; set; }


        [Attr]
        public string PlanIntegrationId { get; set; }


        [Attr]
        public int PlanQuantity { get; set; }


        [Attr]
        public int PlanUnitPrice { get; set; }


        [Attr]
        public int BillingPeriod { get; set; }


        [Attr]
        public string BillingPeriodUnit { get; set; }


        [Attr]
        public int PlanFreeQuantity { get; set; }


        [Attr]
        public string Status { get; set; }


        [Attr]
        public int TrialStart { get; set; }


        [Attr]
        public int TrialEnd { get; set; }


        [Attr]
        public int CurrentTermStart { get; set; }


        [Attr]
        public int CurrentTermEnd { get; set; }


        [Attr]
        public int CreatedAt { get; set; }


        [Attr]
        public int StartedAt { get; set; }


        [Attr]
        public int? ActivatedAt { get; set; }


        [Attr]
        public int? CancelledAt { get; set; }


        [Attr]
        public int? UpdatedAt { get; set; }

        [Attr]
        public int? PauseDate { get; set; }


        [Attr]
        public long ResourceVersion { get; set; }


        [Attr]
        public bool Deleted { get; set; }

        [Attr]
        public string CurrencyCode { get; set; }

        public ICollection<SubscriptionAddOn> SubscriptionAddOns { get; set; }

        [HasManyThrough(nameof(SubscriptionAddOns))]
        [NotMapped]
        public ICollection<AddOn> AddOns { get; set; }

        [Attr]
        public int DueInvoicesCount { get; set; }

        [Attr]
        public Guid? UserId { get; set; }

        [Attr]
        public string UserEmail { get; set; }

        [Attr]
        public Guid? EventOrganiserId { get; set; }

        [HasOne]
        public EventOrganiser EventOrganiser { get; set; }
    }
}