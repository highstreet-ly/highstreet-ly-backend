namespace Highstreetly.Infrastructure.Events
{
    public interface IPaymentIntentFailed: IStripeEvent
    {
        // payment_intent.payment_failed
    }
}