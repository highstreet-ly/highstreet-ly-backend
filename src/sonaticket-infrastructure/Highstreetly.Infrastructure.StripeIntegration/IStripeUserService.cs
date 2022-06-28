using System;
using System.Threading.Tasks;
using Highstreetly.Permissions.Contracts.Requests;

namespace Highstreetly.Infrastructure.StripeIntegration
{
    public interface IStripeUserService
    {
        Task<string> EnsureUserExistsOnStripe(string email);
        Task<string> EnsureUserExistsOnStripe(Guid userId);

        Task<string> EnsureUserExistsOnStripe(User user);
    }
}