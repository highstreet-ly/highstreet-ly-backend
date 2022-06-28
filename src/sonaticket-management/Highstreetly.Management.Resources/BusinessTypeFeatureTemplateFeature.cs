using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("BusinessTypeFeatureTemplateFeatures", Schema = "Management")]
    public class BusinessTypeFeatureTemplateFeature
    {
        public Guid BusinessTypeFeatureTemplateId { get; set; }

        [HasOne]
        public BusinessTypeFeatureTemplate BusinessTypeFeatureTemplate { get; set; }

        public Guid FeatureId { get; set; }

        [HasOne]
        public Feature Feature { get; set; }
    }
}