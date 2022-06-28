namespace Highstreetly.Reservations.Contracts.Requests
{
    public enum States
    {
        PendingReservation = 0,
        PartiallyReserved = 1,
        ReservationCompleted = 2,
        Rejected = 3,
        Confirmed = 4,
        Expired = 5
    }
}