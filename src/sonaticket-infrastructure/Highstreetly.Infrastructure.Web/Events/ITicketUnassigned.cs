namespace Highstreetly.Infrastructure.Events
{
    public interface ITicketUnassigned : ISonaticketEvent
    {
         int Position { get; set; }
    }
}