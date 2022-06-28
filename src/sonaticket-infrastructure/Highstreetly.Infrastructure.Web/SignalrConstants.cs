namespace Highstreetly.Infrastructure
{
    public static class SignalrConstants
    {
        public const string OrderExpired = "order-expired";
        public const string OrderConfirmed = "order-confirmed";
        public const string OrderPlaced = "order-placed";
        public const string PaymentProcessed = "payment-processed";
        public const string OrderProcessing = "order-processing";
        public const string OrderProcessingComplete = "order-processing-complete";
        public const string OrderPriced = "order-priced";
        public const string OrderRefunded = "order-refunded";
        public const string OrderUpdated = "order-updated";

        /*
            Pending = 0,
            Priced = 1,
            Paid = 2,
            Processing = 3,
            Expired = 4,
            ProcessingComplete = 5,
            Refunded = 6
         */
    }
}