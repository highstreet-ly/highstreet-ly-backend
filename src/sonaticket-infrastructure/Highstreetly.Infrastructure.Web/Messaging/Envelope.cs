using System;
using MassTransit;

namespace Highstreetly.Infrastructure.Messaging
{
    /// <summary>
    /// Provides the envelope for an object that will be sent to a bus.
    /// </summary>
    public class Envelope<T> : Envelope, CorrelatedBy<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Envelope{T}"/> class.
        /// </summary>
        public Envelope(T body)
        {
            Body = body;
        }

        /// <summary>
        /// Gets the body.
        /// </summary>
        public T Body { get; private set; }

        /// <summary>
        /// Gets or sets the delay for sending, enqueing or processing the body.
        /// </summary>
        // public TimeSpan Delay { get; set; }

        /// <summary>
        /// Gets or sets the time to live for the message in the queue.
        /// </summary>
        public TimeSpan TimeToLive { get; set; }

        /// <summary>
        /// Gets the correlation id.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets the correlation id.
        /// </summary>
        public string MessageId { get; set; }


        public static implicit operator Envelope<T>(T body)
        {
            return Create(body);
        }
    }
}