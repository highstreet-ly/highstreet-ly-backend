namespace Highstreetly.Management.Contracts
{
    public enum OrderStatus : uint
    {
        Pending = 0,
        Priced = 1,
        Paid = 2,
        Processing = 3,
        Expired = 4,
        ProcessingComplete = 5,
        // Refunded = 6,
        // PartiallyRefunded = 7,
    }
}