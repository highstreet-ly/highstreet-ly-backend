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
    [Table("TicketTypeConfiguration", Schema = "Management")]
    [Resource("ticket-type-configurations")]
    public class TicketTypeConfiguration : IIdentifiable<Guid>, ITicketType, IHasResourceMetadata
    {
        public TicketTypeConfiguration()
        {
            ProductExtraGroups = new List<ProductExtraGroup>();
            Metadata = new Dictionary<string, string>();
            Images = new List<Image>();
        }

        [HasMany]
        public List<Image> Images { get; set; }

        [Attr]
        [NotMapped]
        public string StringId { get => Id.ToString(); set => Id = Guid.Parse(value); }

        [NotMapped] public string LocalId { get; set; }

        [Attr]
        public Guid Id { get; set; }

        [Attr]
        public Guid EventInstanceId { get; set; }

        public Guid? ProductCategoryId { get; set; }

        [HasOne]
        public ProductCategory ProductCategory { get; set; }

        [Attr]
        public string Name { get; set; }

        [Attr]
        public string NormalizedName
        {
            get => string.IsNullOrWhiteSpace(Name) ? "" : Name.ToUpper();
            set { }
        }

        [NotMapped]
        [Attr]
        public string EventSlug { get; set; }

        [Attr]
        public string Description { get; set; }

        [Attr] public long? Price { get; set; } = 0;

        [Attr]
        public bool FreeTier { get; set; }

        [Attr] public int? Quantity { get; set; } = 0;

        [Attr, NotMapped] public int? AddQuantity { get; set; } = 0;

        [Attr] public int TicketsAvailabilityVersion { get; set; } = 0;

        [Attr] public int? AvailableQuantity { get; set; } = 0;

        [Attr]
        public string MainImageId { get; set; }

        [Attr]
        public string Tags { get; set; }

        [Attr]
        public string Group { get; set; }

        [Attr]
        public bool IsPublished { get; set; }

        [Attr]
        public int? SortOrder { get; set; }


        [HasMany]
        public List<ProductExtraGroup> ProductExtraGroups { get; set; }

        [Column("Metadata", TypeName = "jsonb")]
        public string MetadataDB { get; set; }

        [NotMapped]
        [Attr]
        public Dictionary<string, string> Metadata
        {
            get => !string.IsNullOrWhiteSpace(MetadataDB) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(MetadataDB) : new Dictionary<string, string>();
            set => MetadataDB = JsonConvert.SerializeObject(value);
        }
    }
}