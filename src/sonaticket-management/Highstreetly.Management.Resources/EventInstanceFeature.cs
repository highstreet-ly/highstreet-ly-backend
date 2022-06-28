using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("EventInstanceFeatures", Schema = "Management")]
    public class EventInstanceFeature
    {
        public Guid EventInstanceId { get; set; }

        [HasOne]
        public EventInstance EventInstance { get; set; }

        public Guid FeatureId { get; set; }

        [HasOne]
        public Feature Feature { get; set; }
    }
}