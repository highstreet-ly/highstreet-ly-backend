using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Reservations.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Api.Attributes
{
    public class PricedOrderAuthorizationHandler : AuthorizationHandler<AdminOrOwnsPricedOrderRequirement, PricedOrder>
    {
        private readonly ILogger<PricedOrderAuthorizationHandler> _logger;
        private readonly IJwtService _jwtService;
        private readonly IIdentityService _identityService;
        private readonly IJsonApiClient<EventInstance, Guid> _eventInstanceClient;
        
        public PricedOrderAuthorizationHandler(
            ILogger<PricedOrderAuthorizationHandler> logger, 
            IJwtService jwtService, IIdentityService identityService, IJsonApiClient<EventInstance, Guid> eventInstanceClient)
        {
            _logger = logger;
            _jwtService = jwtService;
            _identityService = identityService;
            _eventInstanceClient = eventInstanceClient;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AdminOrOwnsPricedOrderRequirement requirement,
            PricedOrder resource)
        {
            _logger.LogInformation("Entering PricedOrderAuthorizationHandler");

            var token = _jwtService.GetAccessToken();

            // user owns resource
            if (resource.OwnerId != Guid.Empty && context.User.FindFirstValue("sub") == resource.OwnerId.ToString())
            {
                context.Succeed(requirement);
                return;
            }
            // user is a backend service
            else if (context.User.FindFirstValue("access-all") != null)
            {
                context.Succeed(requirement);
                return;
            }

            else if (!string.IsNullOrEmpty(token))
            {
                if (await _jwtService.ValidateTokenAsync(token, (claimsPrincipal) =>
                {
                    var claimOrderId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "order-id");
                    if (claimOrderId == null)
                    {
                        return false;
                    }

                    return claimOrderId.Value == resource.OrderId.ToString();
                }))
                {
                    context.Succeed(requirement);
                    return;
                }
                // user is admin
                else if (await _identityService.UserIsInRoleAsync("Admin", context.User))
                {
                    _logger.LogInformation("PASSED as user admin");
                    context.Succeed(requirement);
                    return;
                }
            }
            
            // there is no case for allowing eoid in
            // I am not sure when this would be needed
            // this should be the same for draft orders
            // only the creator and the backend should need this access
            // afaik
        }
    }
}