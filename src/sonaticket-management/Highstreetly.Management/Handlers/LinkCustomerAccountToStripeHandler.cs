using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.StripeIntegration;
using MassTransit;

namespace Highstreetly.Management.Handlers
{
    public class LinkCustomerAccountToStripeHandler :
        IConsumer<ILinkCustomerAccountToStripe>
    {
        private readonly IStripeUserService _stripeUserService;

        public LinkCustomerAccountToStripeHandler(IStripeUserService stripeUserService)
        {
            _stripeUserService = stripeUserService;
        }

        public Task Consume(ConsumeContext<ILinkCustomerAccountToStripe> context)
        {
            return Task.CompletedTask;
            // var customerId = await _stripeUserService.EnsureUserExistsOnStripe(context.Message.CustomerId);
            //
            // await context.Publish<ICustomerLinkedToStripe>(new CustomerLinkedToStripe
            // {
            //     PaymentIntentId = context.Message.PaymentIntentId,
            //     StripeCustomerId = customerId,
            //     EventOrganiserId = context.Message.EventOrganiserId
            // });
        }
    }
}