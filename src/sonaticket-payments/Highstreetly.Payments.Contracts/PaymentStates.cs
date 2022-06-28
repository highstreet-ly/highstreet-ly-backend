namespace Highstreetly.Payments
{
    public enum PaymentStates
    {
        Initiated = 0,
        Accepted = 1,
        Completed = 2,
        Rejected = 3,
        Refunded = 4,
        PartiallyRefunded = 5,
    }
}