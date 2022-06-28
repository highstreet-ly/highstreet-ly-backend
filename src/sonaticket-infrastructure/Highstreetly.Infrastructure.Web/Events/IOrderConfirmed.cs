namespace Highstreetly.Infrastructure.Events
{
    public interface IOrderConfirmed : ISonaticketEvent
    {
        string Email { get; set; }
    }
}