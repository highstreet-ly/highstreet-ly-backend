using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Highstreetly.Infrastructure;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using Newtonsoft.Json;

namespace Highstreetly.Management.Resources
{
    [Table("TicketType", Schema = "Management")]
    [Resource("ticket-types")]
    public class TicketType : IIdentifiable<Guid>, ITicketType, IHasResourceMetadata
    {
        public TicketType()
        {
            ProductExtraGroups = new List<ProductExtraGroup>();
            Metadata = new Dictionary<string, string>();
            Images = new List<Image>();
        }

        [Key]
        [Attr]
        public Guid Id { get; set; }

        [Attr]
        public Guid EventInstanceId { get; set; }

        public Guid? ProductCategoryId { get; set; }

        [HasOne]
        public  ProductCategory ProductCategory { get; set; }

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

        [Attr]
        public long? Price { get; set; }

        [Attr]
        public bool FreeTier { get; set; }

        [Attr]
        public int? Quantity { get; set; }

        [Attr] public int? AvailableQuantity { get; set; } = 0;

        [Attr]
        public int TicketsAvailabilityVersion { get; set; }

        [Attr]
        public string MainImageId { get; set; }

        [Attr]
        public string Tags { get; set; }

        [Attr]
        public string Group { get; set; }

        [Attr]
        public bool IsPublished { get; set; }

        [NotMapped]
        public string StringId { get => Id.ToString(); set => Id = Guid.Parse(value); }

        [NotMapped] public string LocalId { get; set; }

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

        [HasMany]
        public List<Image> Images { get; set; }
    }
}