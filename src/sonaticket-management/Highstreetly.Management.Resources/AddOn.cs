using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("AddOn", Schema = "Management")]
    [Resource("add-ons")]
    public class AddOn : Identifiable<Guid>
    {
        public AddOn()
        {
            Plans = new List<Plan>();
            Subscriptions = new List<Subscription>();
            Features = new List<Feature>();
        }

        public ICollection<AddOnFeature> AddOnFeatures { get; set; }

        [HasManyThrough(nameof(AddOnFeatures))]
        [NotMapped]
        public ICollection<Feature> Features { get; set; }

        public ICollection<PlanAddOn> PlanAddOns { get; set; }

        [HasManyThrough(nameof(PlanAddOns))]
        [NotMapped]
        public ICollection<Plan> Plans { get; set; }

        public ICollection<SubscriptionAddOn> SubscriptionAddOns { get; set; }

        [HasManyThrough(nameof(SubscriptionAddOns))]
        [NotMapped]
        public ICollection<Subscription> Subscriptions { get; set; }

        [Attr]
        public string IntegrationId { get; set; }

        [Attr]
        public string Name { get; set; }

        [Attr]
        public string NormalizedName
        {
            get => string.IsNullOrWhiteSpace(Name) ? "" : Name.ToUpper();
            set { }
        }


        [Attr]
        public string PricingModel { get; set; }

        [Attr]
        public int Price { get; set; }

        [Attr]
        public string Status { get; set; }

        [Attr]
        public string ChargeType { get; set; }

        [Attr]
        public string CurrencyCode { get; set; }

        [Attr]
        public int Period { get; set; }

        [Attr]
        public string PeriodUnit { get; set; }

        [Attr]
        public bool EnabledInPortal { get; set; }

        [Attr]
        public bool IsShippable { get; set; }

        [Attr]
        public bool ShowDescriptionInInvoices { get; set; }

        [Attr]
        public bool ShowDescriptionInQuotes { get; set; }

        [Attr]
        public string Description { get; set; }

        [Attr]
        public bool? Deleted { get; set; }
    }
}