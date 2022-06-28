using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("ProductExtra", Schema = "Management")]
    public class ProductExtra : Identifiable<Guid>
    {
        [Attr]
        public Guid? TicketTypeId { get; set; }

        [HasOne]
        public TicketType TicketType { get; set; }

        [Attr]
        public Guid? OrderTicketDetailsId { get; set; }

        [HasOne]
        public OrderTicketDetails OrderTicketDetails { get; set; }
        
        [Attr]
        public Guid? ProductExtraGroupId { get; set; }

        [HasOne]
        public ProductExtraGroup ProductExtraGroup { get; set; }

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
        public long Price { get; set; }

        [Attr]
        public bool Selected { get; set; }

        [Attr]
        public long ItemCount { get; set; } = 0;

        [Attr]
        public int? SortOrder { get; set; }
    }
}