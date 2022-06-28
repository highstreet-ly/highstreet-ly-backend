using System;
using System.ComponentModel.DataAnnotations.Schema;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace Highstreetly.Management.Resources
{
    [Table("Image", Schema = "Management")]
    [Resource("images")]
    public class Image : IIdentifiable<Guid>
    {
        [Attr]
        [NotMapped]
        public string StringId { get => Id.ToString(); set => Id = Guid.Parse(value); }

        [NotMapped] public string LocalId { get; set; }

        [Attr]
        public Guid Id { get; set; }

        [Attr]
        public Guid? EventSeriesId { get; set; }

        [HasOne]
        public EventSeries EventSeries { get; set; }

        [Attr]
        public Guid? EventInstanceId { get; set; }

        [HasOne]
        public EventInstance EventInstance { get; set; }

        [Attr]
        public Guid? TicketTypeId { get; set; }

        [HasOne]
        public TicketType TicketType { get; set; }

        [Attr]
        public Guid? TicketTypeConfigurationId { get; set; }

        [HasOne]
        public TicketTypeConfiguration TicketTypeConfiguration { get; set; }

        [Attr]
        public string ExternalImageId { get; set; }

        [HasOne]
        public EventOrganiser EventOrganiser { get; set; }

        [Attr]
        public Guid? EventOrganiserId { get; set; }


        [HasOne]
        public ProductCategory ProductCategory { get; set; }

        [Attr]
        public Guid? ProductCategoryId { get; set; }


    }
}