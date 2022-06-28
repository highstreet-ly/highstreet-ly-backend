using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("Feature", Schema = "Management")]
    [Resource("features")]
    public class Feature : Identifiable<Guid>
    {
        [Attr]
        public string Name { get; set; }
        
        [Attr]
        public string FeatureType { get; set; }

        [Attr]
        public string NormalizedName
        {
            get => string.IsNullOrWhiteSpace(Name) ? "" : Name.ToUpper();
            set { }
        }

        [Attr]
        public string Description { get; set; }

        [Attr]
        public string ClaimValue { get; set; }

        [Attr]
        public bool? Deleted { get; set; }

        public ICollection<PlanFeature> PlanFeatures { get; set; }

        [HasManyThrough(nameof(PlanFeatures))]
        [NotMapped]
        public ICollection<Plan> Plans { get; set; }

        public ICollection<AddOnFeature> AddOnFeatures { get; set; }

        [HasManyThrough(nameof(AddOnFeatures))]
        [NotMapped]
        public ICollection<AddOn> AddOns { get; set; }
        
        public ICollection<EventInstanceFeature> EventInstanceFeatures { get; set; }

        [HasManyThrough(nameof(EventInstanceFeatures))]
        [NotMapped]
        public ICollection<EventInstance> EventInstances { get; set; }
        
        public List<BusinessTypeFeatureTemplateFeature> BusinessTypeFeatureTemplateFeatures { get; set; }

        [HasManyThrough(nameof(BusinessTypeFeatureTemplateFeatures))]
        [NotMapped]
        public ICollection<BusinessTypeFeatureTemplate> BusinessTypeFeatureTemplates { get; set; }

        [Attr]
        public int? SortOrder { get; set; }
    }
}