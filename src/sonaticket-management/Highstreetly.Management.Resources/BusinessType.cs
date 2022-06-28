using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("BusinessType", Schema = "Management")]
    [Resource("business-types")]
    public class BusinessType : Identifiable<Guid>
    {
        public BusinessType()
        {
            EventInstances = new List<EventInstance>();
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
        public bool IsPublished { get; set; }

        [HasMany]
        public List<EventInstance> EventInstances { get; set; }
    }
}