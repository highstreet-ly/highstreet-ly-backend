using System;
using System.Collections.Generic;
using System.Linq;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Reservations.Domain
{
    /// <summary>
    ///     Manages the availability of conference seats. Currently there is one <see cref="IMementoOriginator" /> instance per
    ///     conference.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         For more information on the domain, see
    ///         <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258553">Journey chapter 3</see>.
    ///     </para>
    ///     <para>
    ///         Some of the instances of <see cref="IMemento" /> are highly contentious, as there could be several users trying
    ///         to register for the
    ///         same conference at the same time.
    ///     </para>
    ///     <para>
    ///         Because for large conferences a single instance of <see cref="IMemento" /> can contain a big event stream, the
    ///         class implements
    ///         <see cref="TicketsAvailability" />, so that a <see cref="TicketsAvailability" /> object with the objects'
    ///         internal state (a snapshot) can be cached.
    ///         If the repository supports caching snapshots, then the next time an instance of
    ///         <see cref="TicketsAvailability" /> is created, it can pass
    ///         the cached <see cref="TicketsAvailability" /> in the constructor overload, and avoid reading thousands of
    ///         events from the event store.
    ///     </para>
    /// </remarks>
    public class TicketsAvailability : EventSourced
    {
        private readonly Dictionary<Guid, List<TicketQuantity>> _pendingReservations = new();
        private readonly Dictionary<Guid, int> _remainingTickets = new();

        public TicketsAvailability()
        {
            Handles<TicketsAvailabilityCreated>(Apply);
            Handles<AvailableTicketsChanged>(Apply);
            Handles<TicketsReserved>(Apply);
            Handles<TicketReservationCommitted>(Apply);
            Handles<TicketsReservationCancelled>(Apply);
        }

        public TicketsAvailability(
            Guid id,
            Guid correlation)
            : this()
        {
            CorrelationId = correlation;
            Id = id;
            Update<ITicketsAvailabilityCreated>(
                new TicketsAvailabilityCreated
                {
                    SourceId = id,
                    CorrelationId = correlation
                });
        }

        public TicketsAvailability(
            Guid id,
            Guid correlation,
            IEnumerable<ISonaticketEvent> history)
            : this(
                id,
                correlation)
        {
            LoadFromHistory(history);
        }


        public void AddTickets(
            Guid seatType,
            int? quantity,
            OrderTicketDetails ticketDetails)
        {
            Update<IAvailableTicketsChanged>(
                new AvailableTicketsChanged
                {
                    Tickets = new[]
                    {
                        new TicketQuantity(
                            seatType,
                            quantity ?? 0,
                            ticketDetails)
                    },
                    CorrelationId = CorrelationId
                });
        }

        public void RemoveTickets(
            Guid seatType,
            int quantity)
        {
            Update<IAvailableTicketsChanged>(
                new AvailableTicketsChanged
                {
                    Tickets = new[]
                    {
                        new TicketQuantity(
                            seatType,
                            -quantity,
                            null)
                    },
                    CorrelationId = CorrelationId
                });
        }

        /// <summary>
        ///     Requests a reservation for seats.
        /// </summary>
        /// <param name="reservationId">A token identifying the reservation request.</param>
        /// <param name="wantedTickets">The list of seat requirements.</param>
        /// <param name="isStockManaged"></param>
        /// <remarks>The reservation id is not the id for an aggregate root, just an opaque identifier.</remarks>
        public void MakeReservation(
            Guid reservationId,
            IEnumerable<TicketQuantity> wantedTickets,
            bool isStockManaged)
        {
            var wantedList = wantedTickets.ToList();
            var difference = new Dictionary<string, TicketDifference>();
            TicketsReserved reservation;

            if (wantedList.Any(x => !_remainingTickets.ContainsKey(x.TicketType)))
            {
                throw new ArgumentOutOfRangeException("wantedTickets");
            }

            if (isStockManaged)
            {
                foreach (var w in wantedList)
                {
                    var item = GetOrAdd(
                        difference,
                        w.ComparerString);
                    item.Wanted = w.Quantity;
                    item.Remaining = _remainingTickets[w.TicketType];
                    item.ComparerString = w.ComparerString;
                    item.TicketType = w.TicketType;
                    item.TicketDetails = w.TicketDetails;
                }

                if (_pendingReservations.TryGetValue(
                    reservationId,
                    out var existing))
                {
                    foreach (var e in existing)
                    {
                        var ticketDifference = GetOrAdd(
                            difference,
                            e.ComparerString);
                        ticketDifference.Existing = e.Quantity;
                        ticketDifference.ComparerString = e.ComparerString;
                        ticketDifference.TicketType = e.TicketType;
                        ticketDifference.TicketDetails = e.TicketDetails;
                    }
                }

                reservation = new TicketsReserved
                {
                    ReservationId = reservationId,
                    CorrelationId = CorrelationId,
                    ReservationDetails = difference.Select(
                            x =>
                                new TicketQuantity(
                                    x.Value.TicketType,
                                    x.Value.Actual,
                                    x.Value.TicketDetails))
                        .Where(x => x.Quantity != 0)
                        .ToList(),

                    AvailableTicketsChanged = difference.Select(
                            x =>
                                new TicketQuantity(
                                    x.Value.TicketType,
                                    -x.Value.DeltaSinceLast,
                                    x.Value.TicketDetails))
                        .Where(x => x.Quantity != 0)
                        .ToList()
                };
            }
            else
            {
                foreach (var w in wantedList)
                {
                    var item = GetOrAdd(
                        difference,
                        w.ComparerString);
                    item.Wanted = w.Quantity;
                    item.Remaining = int.MaxValue;
                    item.ComparerString = w.ComparerString;
                    item.TicketType = w.TicketType;
                    item.TicketDetails = w.TicketDetails;
                }

                reservation = new TicketsReserved
                {
                    ReservationId = reservationId,
                    CorrelationId = CorrelationId,
                    ReservationDetails = difference.Select(
                            x =>
                                new TicketQuantity(
                                    x.Value.TicketType,
                                    x.Value.Actual,
                                    x.Value.TicketDetails))
                        .Where(x => x.Quantity != 0)
                        .ToList()
                };
            }

            Update<ITicketsReserved>(reservation);
        }

        public void CancelReservation(
            Guid reservationId)
        {
            if (_pendingReservations.TryGetValue(
                reservationId,
                out var reservation))
            {
                Update<ITicketsReservationCancelled>(
                    new TicketsReservationCancelled
                    {
                        ReservationId = reservationId,
                        AvailableTicketsChanged =
                            reservation.Select(
                                    x => new TicketQuantity(
                                        x.TicketType,
                                        x.Quantity,
                                        null))
                                .ToList(),
                        CorrelationId = CorrelationId
                    });
            }
        }

        public void CommitReservation(
            Guid reservationId)
        {
            if (_pendingReservations.ContainsKey(reservationId))
            {
                Update<ITicketReservationCommitted>(
                    new TicketReservationCommitted
                        {ReservationId = reservationId, CorrelationId = CorrelationId});
            }
        }

        private static TValue GetOrAdd<TKey, TValue>(
            Dictionary<TKey, TValue> dictionary,
            TKey key) where TValue : new()
        {
            if (!dictionary.TryGetValue(
                key,
                out var value))
            {
                value = new TValue();
                dictionary[key] = value;
            }

            return value;
        }

        public void Apply(
            AvailableTicketsChanged e)
        {
            foreach (var seat in e.Tickets)
            {
                var newValue = seat.Quantity;
                if (_remainingTickets.TryGetValue(
                    seat.TicketType,
                    out var remaining))
                {
                    newValue += remaining;
                }

                _remainingTickets[seat.TicketType] = newValue;
            }
        }

        public void Apply(
            TicketsReserved e)
        {
            var details = e.ReservationDetails.ToList();
            if (details.Count > 0)
            {
                _pendingReservations[e.ReservationId] = details;
            }
            else
            {
                _pendingReservations.Remove(e.ReservationId);
            }

            // non-stock checked stores don't provide AvailableTicketsChanged
            if (e.AvailableTicketsChanged != null)
            {
                foreach (var ticket in e.AvailableTicketsChanged)
                    _remainingTickets[ticket.TicketType] = _remainingTickets[ticket.TicketType] + ticket.Quantity;
            }
        }

        public void Apply(
            TicketReservationCommitted e)
        {
            _pendingReservations.Remove(e.ReservationId);
        }

        public void Apply(
            TicketsReservationCancelled e)
        {
            _pendingReservations.Remove(e.ReservationId);

            foreach (var ticket in e.AvailableTicketsChanged)
                _remainingTickets[ticket.TicketType] = _remainingTickets[ticket.TicketType] + ticket.Quantity;
        }

        public void Apply(
            TicketsAvailabilityCreated obj)
        {
            // just to get the AR saved ahead of any seats becoming available so we can eliminate race conditions
            // previously we would try and save 2 TicketsAvailability in parallel as each ticket was published
            // we would get 2 events with the same type, sourceid and version which would cause a problem
        }

        /// <summary>
        ///     Saves the object's state to an opaque memento object (a snapshot) that can be used to restore the state by using
        ///     the constructor overload.
        /// </summary>
        /// <returns>An opaque memento object that can be used to restore the state.</returns>
        public IMemento SaveToMemento()
        {
            return new Memento
            {
                Version = Version,
                RemainingSeats = _remainingTickets.ToArray(),
                PendingReservations = _pendingReservations.ToArray()
            };
        }

        public class TicketDifference
        {
            public int Wanted { get; set; }
            public int Existing { get; set; }
            public int Remaining { get; set; }

            public int Actual => Math.Min(
                Wanted,
                Math.Max(
                    Remaining,
                    0) + Existing);

            public int DeltaSinceLast => Actual - Existing;

            public string ComparerString { get; set; }
            public Guid TicketType { get; set; }
            public OrderTicketDetails TicketDetails { get; set; }
        }

        public class Memento : IMemento
        {
            internal KeyValuePair<Guid, int>[] RemainingSeats { get; set; }
            internal KeyValuePair<Guid, List<TicketQuantity>>[] PendingReservations { get; set; }
            public int Version { get; internal set; }
        }
    }
}