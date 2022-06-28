using System.Security.Claims;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using IdentityServer4.Validation;

namespace Highstreetly.Ids.Services
{
    public class PoopRequestValidator : ICustomTokenRequestValidator
    {
        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            var client = context.Result.ValidatedRequest.Client;

            // allow inter-service communication 
            // this is a hack since I can't seem to get client credentials tokens to add claims using the profile service
            if (client.ClientId == Scopes.PaymentApi || client.ClientId == Scopes.PaymentProcessor
                || client.ClientId == Scopes.TicketReservationsApi || client.ClientId == Scopes.TicketReservationsProcessor
                || client.ClientId == Scopes.TicketManagementApi || client.ClientId == Scopes.TicketManagementProcessor
                || client.ClientId == Scopes.PermissionsApi || client.ClientId == Scopes.PermissionsProcessor)
            {
                context.Result.ValidatedRequest.ClientClaims.Add(new Claim("access-all", "true"));

                // don't want it to be prefixed with "client_" ? we change it here (or from global settings)
                context.Result.ValidatedRequest.Client.ClientClaimsPrefix = "";
            }
        }
    }
}