using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Extensions;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using Newtonsoft.Json;

namespace Highstreetly.Management.Resources
{
    [Table("EventInstance", Schema = "Management")]
    [Resource("event-instances")]
    public class EventInstance : Identifiable<Guid>, IHasResourceMetadata, IHasOwner, IHasEventOrganiser
    {
        public EventInstance()
        {
            OpeningTimes = new OpeningTimes();
            Metadata = new Dictionary<string, string>();
            Images = new List<Image>();
            TicketTypes = new List<TicketType>();
            ProductCategories = new List<ProductCategory>();
            Features = new List<Feature>();
            EventInstanceFeatures = new List<EventInstanceFeature>();
        }

        [Attr]
        [Column(TypeName = "jsonb")]
        public OpeningTimes OpeningTimes { get; set; }

        [Attr]
        public Guid? OwnerId { get; set; }

        [Attr]
        public Guid EventSeriesId { get; set; }
        
        [HasOne]
        public EventSeries EventSeries { get; set; }

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
        public string Location { get; set; }

        [Attr]
        public string ShortLocation { get; set; }

        [Attr(PublicName = "postcode")]
        public string PostCode { get; set; }

        [Attr] public int? DeliveryRadiusMiles { get; set; } = 0;

        [Attr] public string Tagline { get; set; }

        [Attr]
        public string MainImageId { get; set; }

        [Attr]
        public string HeroImageId { get; set; }

        [Attr]
        public string Hero2ImageId { get; set; }

        [Attr]
        public string LogoImageId { get; set; }

        [Attr]
        public bool IsPublished { get; set; }

        [Attr]
        public bool? IsToTable { get; set; }

        [Attr]
        public bool IsClickAndCollect { get; set; }

        [Attr]
        public bool IsLocalDelivery { get; set; }

        [Attr]
        public bool IsNationalDelivery { get; set; }

        [Attr]
        public bool ShowWaitingList { get; set; }

        [Attr]
        public DateTime? DateCreated { get; set; }

        [Attr]
        public string Category { get; set; }

        [Attr]
        public string Lat { get; set; }

        [Attr]
        public string Lng { get; set; }

        [Attr]
        public string Slug
        {
            get
            {
                var result = "";

                if (!string.IsNullOrEmpty(Name))
                {
                    result += Name;
                }

                if (!string.IsNullOrEmpty(Location))
                {
                    result += $"-{Location}";
                }

                return result.GenerateSlug();
            }
            set { }
        }

        [Attr]
        public bool WasEverPublished { get; set; }

        [Attr]
        public bool Featured { get; set; }
        
        [HasMany]
        public List<TicketType> TicketTypes { get; set; }
        
        [HasMany]
        public List<Image> Images { get; set; }

        [Attr] public int? OrdersConfirmed { get; set; } = 0;

        [Attr]
        public string SupportEmail { get; set; }

        [Attr]
        public string SupportPhone { get; set; }

        [Attr]
        public string NotificationEmail { get; set; }

        [Attr]
        public string NotificationPhone { get; set; }

        [Attr]
        public bool Deleted { get; set; }
        
        /// <summary>
        /// TODO: this can be a computed property but will need to always include features
        /// </summary>
        [Attr]
        public bool IsOpen { get; set; }

        [Attr]
        public Guid EventOrganiserId { get; set; }

        [HasOne]
        public EventOrganiser EventOrganiser { get; set; }

        [Attr] 
        public long NationalDeliveryFlatFee { get; set; } = 0;

        [Attr] 
        public long NationalDeliveryFlatFeeFreeAfter { get; set; } = 0;

       
        [Column("Metadata", TypeName = "jsonb")]
        public string MetadataDB { get; set; }

        [Attr]
        [NotMapped]
        public Dictionary<string, string> Metadata
        {
            get => !string.IsNullOrWhiteSpace(MetadataDB) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(MetadataDB) : new Dictionary<string, string>();
            set => MetadataDB = JsonConvert.SerializeObject(value);
        }

        [Attr] public int? PaymentPlatformFeePaidBy { get; set; } = 2;

        [Attr] public int? PlatformFeePaidBy { get; set; } = 2;

        [HasMany]
        public List<ProductCategory> ProductCategories { get; set; }
        
        public ICollection<EventInstanceFeature> EventInstanceFeatures { get; set; }
        
        [HasManyThrough(nameof(EventInstanceFeatures))]
        [NotMapped]
        public List<Feature> Features { get; set; }

        [Attr]
        public Guid? BusinessTypeId { get; set; }

        [HasOne]
        public BusinessType BusinessType { get; set; }
        
        [Attr]
        public bool IsStockManaged { get; set; }
    }
}

