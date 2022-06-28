using System;

namespace Highstreetly.Reservations.ReadModel
{
    public class PricedOrderLineSeatTypeDescription
    {
        [Marten.Schema.Identity]
        public Guid SeatTypeId { get; set; }
        public string Name { get; set; }
    }
}