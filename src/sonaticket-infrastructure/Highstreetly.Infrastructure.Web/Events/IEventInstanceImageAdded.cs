namespace Highstreetly.Infrastructure.Events
{
    public interface IEventInstanceImageAdded : ISonaticketEvent
    {
        bool IsMainIMage { get; set; }
        string ImageId { get; set; }
    }
}