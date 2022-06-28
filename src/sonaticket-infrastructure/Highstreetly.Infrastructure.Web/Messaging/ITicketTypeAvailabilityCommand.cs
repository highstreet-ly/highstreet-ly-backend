using System;

namespace Highstreetly.Infrastructure.Messaging
{
    public interface ITicketTypeAvailabilityCommand : ICommand, IMessageSessionProvider
    {
        Guid EventInstanceId { get; set; }
    }

    /// <summary>
    /// Static factory class for <see cref="Envelope{T}"/>.
    /// </summary>
    public abstract class Envelope
    {
        /// <summary>
        /// Creates an envelope for the given body.
        /// </summary>
        public static Envelope<T> Create<T>(T body)
        {
            return new Envelope<T>(body);
        }
    }
}