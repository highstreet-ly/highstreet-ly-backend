using System;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public interface IEventInstanceEvent : ISonaticketEvent
    {
        Guid EventSeriesId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string PostCode { get; set; }
        string Location { get; set; }
        int? DeliveryRadiusMiles { get; set;  }
        string Lat { get; set; }
        string Lng { get; set; }
        string ShortLocation {get; set; }
        string Slug { get; set; }
        string Tagline { get; set; }
        string TwitterSearch { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        Owner Owner { get; set; }
        string Category { get; set; }
        Guid EventOrganiserId { get; set; }
        
        string HeroImageId { get; set; }
        string Hero2ImageId { get; set; }
    }
}
