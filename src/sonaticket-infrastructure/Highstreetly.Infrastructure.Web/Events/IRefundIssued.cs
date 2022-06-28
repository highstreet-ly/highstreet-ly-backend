namespace Highstreetly.Infrastructure.Events
{
    public interface IRefundIssued : ISonaticketEvent
    {
        int Amount { get; set; }
    }
}