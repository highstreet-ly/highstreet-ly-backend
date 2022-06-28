using System;
using System.Collections.Generic;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Highstreetly.Infrastructure;
using Newtonsoft.Json;

namespace Highstreetly.Management.Resources
{
    [Table("EventSeries", Schema = "Management")]
    [Resource("event-series")]
    public class EventSeries : Identifiable<Guid>, IHasResourceMetadata, IHasEventOrganiser, IHasOwner
    {
        public EventSeries()
        {
            EventInstances = new List<EventInstance>();
            Metadata = new Dictionary<string, string>();
            Images = new List<Image>();
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
        public string DescriptionHtml { get; set; }

        [Attr]
        public string Slug
        {
            get
            {
                var result = "";

                if (!string.IsNullOrEmpty(Name))
                {
                    result += Name;
                }

                return result;
            }
            set
            {

            }
        }

        [Attr]
        public Guid EventOrganiserId { get; set; }

        [HasOne]
        public EventOrganiser EventOrganiser { get; set; }

        [Attr]
        public string Category { get; set; }

        [Attr]
        public bool WasEverPublished { get; set; }

        [Attr]
        public string OwnerName { get; set; }

        [Attr]
        public Guid? OwnerId { get; set; }

        [Attr]
        public string OwnerEmail { get; set; }

        [Attr]
        public string Tagline { get; set; }

        [Attr]
        public bool IsPublished { get; set; }

        [Attr]
        public bool Featured { get; set; }

        [Attr]
        public string MainImageId { get; set; }

        [Attr]
        public string HeroImageId { get; set; }

        [Attr]
        public string Hero2ImageId { get; set; }

        [Attr]
        public string LogoImageId { get; set; }

        [HasMany]
        public List<EventInstance> EventInstances { get; set; }

        [Attr]
        [NotMapped]
        public int? EventCount { get; set; }

        [Attr]
        [NotMapped]
        public bool Onboarding { get; set; }

        [Column("Metadata", TypeName = "jsonb")]
        public string MetadataDB { get; set; }

        [NotMapped]
        [Attr]
        public Dictionary<string, string> Metadata
        {
            get => !string.IsNullOrWhiteSpace(MetadataDB) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(MetadataDB) : new Dictionary<string, string>();
            set => MetadataDB = JsonConvert.SerializeObject(value);
        }

        [HasMany]
        public List<Image> Images { get; set; }
    }
}