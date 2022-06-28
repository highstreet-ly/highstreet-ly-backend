using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("ProductExtraGroup", Schema = "Management")]
    public class ProductExtraGroup : Identifiable<Guid>
    {
        public ProductExtraGroup()
        {
            ProductExtras = new List<ProductExtra>();
        }

        [HasMany]
        public List<ProductExtra> ProductExtras { get; set; }
        
        [Attr]
        public int MaxSelectable { get; set; }
        
        [Attr]
        public int MinSelectable { get; set; }
        
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
        
        [HasOne] 
        public TicketTypeConfiguration TicketTypeConfiguration { get; set; }
        
        [Attr] 
        public Guid? TicketTypeConfigurationId { get; set; }
        
        [HasOne]
        public TicketType TicketType { get; set; }
        
        [Attr]
        public Guid? TicketTypeId { get; set; }

        [Attr]
        public int? SortOrder { get; set; }
    }
}