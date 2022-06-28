using System;
using System.Collections.Generic;

namespace Highstreetly.Infrastructure.EventSourcing
{
    /// <summary>
    /// Base class for event sourced entities that implements <see cref="IEventSourced"/>. 
    /// </summary>
    /// <remarks>
    /// <see cref="IEventSourced"/> entities do not require the use of <see cref="EventSourced"/>, but this class contains some common 
    /// useful functionality related to versions and rehydration from past events.
    /// </remarks>
    public abstract class EventSourced : IEventSourced
    {
        private Dictionary<Type, Action<ISonaticketEvent>> handlers = new Dictionary<Type, Action<ISonaticketEvent>>();
        private Dictionary<Type, ISonaticketEvent> pendingEvents = new Dictionary<Type, ISonaticketEvent>();

        private Guid id;
        private int version = 0;

        public EventSourced()
        {

        }

        public EventSourced(Guid id)
        {
            this.id = id;
        }

        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets the entity's version. As the entity is being updated and events being generated, the version is incremented.
        /// </summary>
        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// Gets the collection of new events since the entity was loaded, as a consequence of command handling.
        /// </summary>
        public Dictionary<Type, ISonaticketEvent> Events
        {
            get { return pendingEvents; }
            set { pendingEvents = value; }
        }

        public void ClearUncommittedEvents()
        {
            pendingEvents.Clear();
        }

        protected void Handles<TEvent>(Action<TEvent> handler)
            where TEvent : ISonaticketEvent
        {
            handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
        }

        public void Update<T>(ISonaticketEvent e)
        {
            try
            {
                if (!typeof(T).IsInterface)
                {
                    throw new ArgumentException("T must be an interface");
                }

                e.SourceId = Id;
                e.Version = version + 1;
                ApplyEvent(e);
                pendingEvents.Add(e.GetType().GetInterfaces()[0], e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected void ApplyEvent(ISonaticketEvent @event)
        {
            try
            {
                handlers[@event.GetType()].Invoke(@event);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            version++;
        }

        protected void LoadFromHistory(IEnumerable<ISonaticketEvent> history)
        {
            foreach (var sonaticketEvent in history)
            {
                this.ApplyEvent(sonaticketEvent);
            }
        }

        public Guid CorrelationId { get; set; }
    }
}
