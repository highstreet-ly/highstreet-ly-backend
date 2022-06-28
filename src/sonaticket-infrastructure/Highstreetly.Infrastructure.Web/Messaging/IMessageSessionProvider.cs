namespace Highstreetly.Infrastructure.Messaging
{
    public interface IMessageSessionProvider
    {
        string SessionId { get; }
    }
}