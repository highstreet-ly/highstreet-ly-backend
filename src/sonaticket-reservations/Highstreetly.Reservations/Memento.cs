using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Infrastructure.MessageDtos;

namespace Highstreetly.Reservations
{
    public class Memento : IMemento
    {
        public int Version { get; internal set; }
        internal KeyValuePair<Guid, int>[] RemainingSeats { get; set; }
        internal KeyValuePair<Guid, List<TicketQuantity>>[] PendingReservations { get; set; }
    }
}