using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Permissions.Contracts.Requests;
using Stripe;

namespace Highstreetly.Infrastructure.StripeIntegration
{
    public class StripeUserService : IStripeUserService
    {
        private readonly IIdentityService _identityService;
        private const string StripeCustomerId = "stripe-customer-id";
        private readonly IJsonApiClient<User, Guid> _userApiClient;

        public StripeUserService(Configuration.StripeConfiguration stripeConfiguration, IIdentityService identityService, IJsonApiClient<User, Guid> userApiClient)
        {
            _identityService = identityService;
            _userApiClient = userApiClient;
            StripeConfiguration.ApiKey = stripeConfiguration.ApiKey;
        }

        public async Task<string> EnsureUserExistsOnStripe(string email)
        {
            var queryBuilder = new QueryBuilder()
                .Equalz("normalized-email", email.ToUpper());

            var usersQuery = await _userApiClient.GetListAsync(queryBuilder);

            var users = usersQuery as User[] ?? usersQuery.ToArray();

            if (!users.Any())
            {
                throw new Exception("User not found");
            }

            var user = users.First();

            return await EnsureUserExistsOnStripe(user);
        }

        public async Task<string> EnsureUserExistsOnStripe(Guid userId)
        {
            var user = await _userApiClient.GetAsync(userId);
            return await EnsureUserExistsOnStripe(user);
        }

        public async Task<string> EnsureUserExistsOnStripe(User user)
        {
            var email = user.Email;

            var customerService = new CustomerService();
            var stripeSearch = await customerService.ListAsync(new CustomerListOptions
            {
                Email = email
            });

            if (!stripeSearch.Data.Any())
            {
                var customerCreateOptions = new CustomerCreateOptions
                {
                    Email = email,
                };

                var customer = await customerService.CreateAsync(customerCreateOptions);

                var claim = new Claim
                {
                    ClaimType = StripeCustomerId,
                    ClaimValue = customer.Id
                };

                await _identityService.AddUserClaimAsync(user, claim);

                return customer.Id;
            }

            return stripeSearch.First().Id;
        }
    }
}
