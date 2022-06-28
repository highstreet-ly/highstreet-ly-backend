using System.Threading.Tasks;
using Highstreetly.Management.Resources;
using Highstreetly.Reservations.Contracts.Requests;

namespace Highstreetly.Management
{
    public interface INotificationSenderService
    {
        Task SendOrderConfirmedOperatorAsync(
            EventInstance evt,
            PricedOrder pricedOrder,
            Order order);

        Task SendOrderConfirmedAsync(
            string email,
            EventInstance evt,
            PricedOrder pricedOrder,
            Order order);

        Task SendOrderRefundedEmail(
            string email,
            EventInstance evt,
            PricedOrder pricedOrder,
            string hrOrderId,
            string url,
            long refundedAmount);

        Task SendOrderProcessingCompleteAsync(
            string email,
            EventInstance evt,
            PricedOrder pricedOrder,
            Order order);

        Task SendOrderProcessingAsync(
            string email,
            EventInstance evt,
            PricedOrder pricedOrder,
            Order order);

        bool ValidatePhoneNumber(
            string number);
    }
}