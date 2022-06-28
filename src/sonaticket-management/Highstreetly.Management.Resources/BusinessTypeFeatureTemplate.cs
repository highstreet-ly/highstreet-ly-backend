using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("BusinessTypeFeatureTemplates", Schema = "Management")]
    public class BusinessTypeFeatureTemplate : Identifiable<Guid>
    {
        public BusinessTypeFeatureTemplate()
        {
            BusinessTypeFeatureTemplateFeatures = new List<BusinessTypeFeatureTemplateFeature>();
            Features = new List<Feature>();
        }

        public Guid BusinessTypeId { get; set; }
        
        [HasOne]
        public BusinessType BusinessType { get; set; }
        
        public List<BusinessTypeFeatureTemplateFeature> BusinessTypeFeatureTemplateFeatures { get; set; }
        
        [HasManyThrough(nameof(BusinessTypeFeatureTemplateFeatures))]
        [NotMapped]
        public List<Feature> Features { get; set; }

        
    }
}