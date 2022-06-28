using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Marten;
using Marten.Events;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Infrastructure.EventSourcing
{
    public class MartenEventStoreRepository<T> : IEventSourcedRepository<T> where T : class, IEventSourced, new()
    {
        private static readonly string SourceType = typeof(T).Name;
        private readonly IDocumentSession _documentSession;
        private readonly MassTransit.IBusControl _eventBus;
        ILogger<MartenEventStoreRepository<T>> _logger;

        public MartenEventStoreRepository(MassTransit.IBusControl eventBus, IDocumentSession documentSession, ILogger<MartenEventStoreRepository<T>> logger)
        {
            _documentSession = documentSession;
            _logger = logger;
            _eventBus = eventBus;
        }

        private static readonly MethodInfo ApplyEvent = typeof(EventSourced).GetMethod("ApplyEvent", BindingFlags.Instance | BindingFlags.NonPublic);


        public T Find(Guid id, int? version = null)
        {

            IReadOnlyList<IEvent> events;
            events = _documentSession.Events.FetchStream(id, version ?? 0);

            if (events != null && events.Any())
            {
                var instance = Activator.CreateInstance(typeof(T), true);

                // Replay our aggregate state from the event stream
                events.Aggregate(instance, (o, @event) =>
                {
                    return ApplyEvent.Invoke(instance, new[] { @event.Data });
                });
                //TODO: this should be set on AR creation in the AR ctor - because each creation should come from an event and that event should set it's ID
                ((T)instance).Id = id;

                return (T)instance;
            }

            return null;
        }

        public T Get(Guid id)
        {
            var entity = Find(id);
            if (entity == null)
            {
                throw new EntityNotFoundException(id, SourceType);
            }

            return entity;
        }

        public async Task Save(T aggregate, string correlationId)
        {
            if (aggregate.Events.Count < 1)
            {
                return;
            }

            try
            {
                foreach (var @event in aggregate.Events)
                {
                    _documentSession.Events.Append(aggregate.Id, /*aggregate.Version,*/ @event.Value);
                }

                await _documentSession.SaveChangesAsync();

                var eventsPairs = aggregate.Events.ToArray();

                foreach (var @event in eventsPairs)
                {
                    await _eventBus.Publish(@event.Value, @event.Key);
                }

                aggregate.ClearUncommittedEvents();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}