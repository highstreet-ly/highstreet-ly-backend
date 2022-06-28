using System;
using System.Threading.Tasks;

namespace Highstreetly.Infrastructure.StripeIntegration
{
    public interface IStripeProductService
    {
        Task EnsureProductExistsOnStripe(
            string productName, 
            Guid ticketTypeConfigId,
            long productPrice);
    }
}