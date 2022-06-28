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
    [Table("EventOrganiser", Schema = "Management")]
    [Resource("event-organisers")]
    public class EventOrganiser : IIdentifiable<Guid>, IHasResourceMetadata
    {
        public EventOrganiser()
        {
            Metadata = new Dictionary<string, string>();
            Images = new List<Image>();
            Subscriptions = new List<Subscription>();
        }

        [Attr]
        public Guid Id { get; set; }

        [Attr]
        public string StripeAccountId { get; set; }

        [Attr]
        public string StripePublishableKey { get; set; }

        [Attr(Capabilities = ~AttrCapabilities.AllowCreate)]
        public string StripeAccessToken { get; set; }

        [Attr]
        public bool IsConnectedToStripe
        {
            get =>
                !string.IsNullOrEmpty(StripeAccountId) && !string.IsNullOrEmpty(StripePublishableKey) &&
                !string.IsNullOrEmpty(StripeAccessToken);
            set
            {
                var x = value;
            }
        }

        [Attr]
        public string StripeLoginLink { get; set; }

        [Attr]
        [NotMapped]
        public string StringId { get => Id.ToString(); set => Id = Guid.Parse(value); }

        [NotMapped] public string LocalId { get; set; }

        [Attr]
        public string StripeCode { get; set; }

        [Attr]
        public string Url { get; set; }

        [Attr]
        public string Description { get; set; }

        [Attr]
        public string Name { get; set; }

        [Attr]
        public string NormalizedName
        {
            get => string.IsNullOrWhiteSpace(Name) ? "" : Name.ToUpper();
            set { }
        }

        [Attr]
        public string LogoId { get; set; }

        [Attr] public long PlatformFee { get; set; } = 0;

        [Attr]
        public ApplicationSchemaType SchemaType { get; set; }

        [HasMany]
        public IList<EventSeries> EventSeries { get; set; }

        [Column("Metadata", TypeName = "jsonb")]
        public string MetadataDB { get; set; }

        [Attr]
        [NotMapped]
        public Dictionary<string, string> Metadata
        {
            get => !string.IsNullOrWhiteSpace(MetadataDB) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(MetadataDB) : new Dictionary<string, string>();
            set => MetadataDB = JsonConvert.SerializeObject(value);
        }

        [HasOne]
        public DashboardStat DashboardStats { get; set; }

        [HasMany]
        public ICollection<Image> Images { get; set; }

        [HasMany]
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}