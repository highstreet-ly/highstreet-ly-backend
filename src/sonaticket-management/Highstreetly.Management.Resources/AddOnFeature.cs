using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("AddOnFeatures", Schema = "Management")]
    public class AddOnFeature
    {
        public Guid AddOnId { get; set; }

        [HasOne]
        public AddOn AddOn { get; set; }

        public Guid FeatureId { get; set; }

        [HasOne]
        public Feature Feature { get; set; }
    }
}