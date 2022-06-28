using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("PlanAddOns", Schema = "Management")]
    public class PlanAddOn
    {
        public Guid PlanId { get; set; }

        [HasOne]
        public Plan Plan { get; set; }

        public Guid AddOnId { get; set; }

        [HasOne]
        public AddOn AddOn { get; set; }
    }
}