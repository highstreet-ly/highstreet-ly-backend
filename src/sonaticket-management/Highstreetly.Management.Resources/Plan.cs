using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Highstreetly.Infrastructure;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using Newtonsoft.Json;

namespace Highstreetly.Management.Resources
{
    [Table("Plan", Schema = "Management")]
    [Resource("plans")]
    public class Plan : Identifiable<Guid>, IHasResourceMetadata
    {
        public Plan()
        {
            AddOns = new List<AddOn>();
            Subscriptions = new List<Subscription>();
        }

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
        public string Description { get; set; }

        [Attr]
        public int? Price { get; set; }

        [Attr]
        public string PricingModel { get; set; }

        public ICollection<PlanAddOn> PlanAddOns { get; set; }

        [HasManyThrough(nameof(PlanAddOns))]
        [NotMapped]
        public ICollection<AddOn> AddOns { get; set; }

        [Attr]
        public string Status { get; set; }

        [Attr]
        public string ChargeModel { get; set; }

        [Attr]
        public string CurrencyCode { get; set; }

        [Attr]
        public bool? EnabledInHostedPages { get; set; }

        [Attr]
        public bool? EnabledInPortal { get; set; }

        [Attr]
        public int? FreeQuantity { get; set; }

        [Attr]
        public int? Period { get; set; }

        [Attr]
        public string PeriodUnit { get; set; }

        [Attr]
        public bool? ShowDescriptionInInvoices { get; set; }

        [Attr]
        public bool? ShowDescriptionInQuotes { get; set; }

        [Attr]
        public bool? Taxable { get; set; }

        [HasMany]
        [NotMapped]
        public ICollection<Subscription> Subscriptions { get; set; }

        [Attr]
        public bool? Deleted { get; set; }

        public ICollection<PlanFeature> PlanFeatures { get; set; }

        [HasManyThrough(nameof(PlanFeatures))]
        [NotMapped]
        public ICollection<Feature> Features { get; set; }

        // product subscriptions:

        public ICollection<PlanTicket> PlanTickets { get; set; }

        [HasManyThrough(nameof(PlanTickets))]
        [NotMapped]
        public ICollection<TicketType> TicketTypes { get; set; }

        [Attr]
        public Guid? EventOrganiserId { get; set; }

        [HasOne]
        public EventOrganiser EventOrganiser { get; set; }

        [Attr]
        public Guid? EventInstanceId { get; set; }

        [HasOne]
        public EventInstance EventInstance { get; set; }

        [Attr]
        public string MainImageId { get; set; }

        [Column("Metadata", TypeName = "jsonb")]
        public string MetadataDB { get; set; }

        [NotMapped]
        [Attr]
        public Dictionary<string, string> Metadata
        {
            get => !string.IsNullOrWhiteSpace(MetadataDB) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(MetadataDB) : new Dictionary<string, string>();
            set => MetadataDB = JsonConvert.SerializeObject(value);
        }

        [Attr]
        public int? SortOrder { get; set; }

        [Attr]
        public bool PubliclyVisible { get; set; }
    }
}