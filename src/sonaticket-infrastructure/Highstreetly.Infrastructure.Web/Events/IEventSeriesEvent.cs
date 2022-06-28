using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Infrastructure.Events
{
    public interface IEventSeriesEvent : ISonaticketEvent
    {
        string Name { get; set; }
        string Description { get; set; }
        string DescriptionHtml {get; set; }
        string Slug { get; set; }
        string Tagline { get; set; }
        string TwitterSearch { get; set; }
        Owner Owner { get; set; }
        string Category { get; set; }
        bool WasEverPublished { get; set; }
    }
}