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
    [Table("ProductCategory", Schema = "Management")]
    [Resource("product-categories")]
    public class ProductCategory : Identifiable<Guid>, IHasResourceMetadata
    {
        public ProductCategory()
        {
            TicketTypeConfigurations = new List<TicketTypeConfiguration>();
            TicketTypes = new List<TicketType>();
            Metadata = new Dictionary<string, string>();
        }

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
        public string MainImageId { get; set; }

        [Attr]
        public int? SortOrder { get; set; }

        [Attr]
        public bool Enabled { get; set; }

        [HasMany]
        public List<Image> Images { get; set; }

        [HasMany]
        public List<TicketType> TicketTypes { get; set; }

        [HasMany]
        public List<TicketTypeConfiguration> TicketTypeConfigurations { get; set; }

        [Column("Metadata", TypeName = "jsonb")]
        public string MetadataDB { get; set; }

        [Attr]
        public Guid EventInstanceId { get; set; }

        [HasOne]
        public EventInstance EventInstance { get; set; }

        [Attr]
        [NotMapped]
        public Dictionary<string, string> Metadata
        {
            get => !string.IsNullOrWhiteSpace(MetadataDB) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(MetadataDB) : new Dictionary<string, string>();
            set => MetadataDB = JsonConvert.SerializeObject(value);
        }
    }
}