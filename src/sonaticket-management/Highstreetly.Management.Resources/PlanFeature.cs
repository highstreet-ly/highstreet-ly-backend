using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("PlanFeatures", Schema = "Management")]
    public class PlanFeature
    {
        public Guid PlanId { get; set; }

        [HasOne]
        public Plan Plan { get; set; }

        public Guid FeatureId { get; set; }

        [HasOne]
        public Feature Feature { get; set; }
    }
}