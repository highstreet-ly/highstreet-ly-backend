using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;

namespace Highstreetly.Reservations.ReadModel
{
    /// <summary>
    /// TODO: might not need
    /// </summary>
    public class DeliveryMethodChangedHandler : IConsumer<IDeliveryMethodChanged>
    {
        private readonly ReservationDbContext _reservationDbContext;
        private readonly IPricingService _pricingService;

        public DeliveryMethodChangedHandler(ReservationDbContext reservationDbContext, IPricingService pricingService)
        {
            _reservationDbContext = reservationDbContext;
            _pricingService = pricingService;
        }


        public Task Consume(ConsumeContext<IDeliveryMethodChanged> context)
        {
            return Task.CompletedTask;
        }
    }
}