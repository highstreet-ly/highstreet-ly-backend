using System.Linq;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.EventSourcing;

namespace Highstreetly.Reservations.Tests
{
    public static class EnumerableEventsExtensions
    {
        public static TEvent SingleEvent<TEvent>(this IEventSourced aggregate)
            where TEvent : ISonaticketEvent
        {
            return (TEvent)aggregate.Events.Select(x=>x.Value).OfType<TEvent>().Single();
        }
    }
}