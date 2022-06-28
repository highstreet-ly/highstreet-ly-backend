using System;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public class EventInstanceUpdated : IEventInstanceUpdated
    {
        public Guid CorrelationId { get; set; }
        public Guid SourceId { get; set; }
        public TimeSpan Delay { get; set; }
        public int Version { get; set; }
        public Guid EventSeriesId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PostCode { get; set; }
        public string Location { get; set; }
        public int? DeliveryRadiusMiles { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string ShortLocation { get; set; }
        public string Slug { get; set; }
        public string Tagline { get; set; }
        public string TwitterSearch { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Owner Owner { get; set; }
        public string Category { get; set; }
        public Guid EventOrganiserId { get; set; }
        public string HeroImageId { get; set; }
        public string Hero2ImageId { get; set; }
    }
}