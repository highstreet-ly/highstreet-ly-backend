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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Api.Attributes
{
    public class DraftOrderAuthorizationHandler : AuthorizationHandler<AdminOrOwnsDraftOrderRequirement, DraftOrder>
    {
        private readonly IMemoryCache _cache;
        private readonly IJwtService _jwtService;
        private readonly ILogger<DraftOrderAuthorizationHandler> _logger;
        private readonly IIdentityService _identityService;
        private readonly IJsonApiClient<EventInstance, Guid> _eventInstanceClient;

        public DraftOrderAuthorizationHandler(
                        IMemoryCache cache,
                        ILogger<DraftOrderAuthorizationHandler> logger,
                        IJwtService jwtService,
                        IIdentityService identityService,
                        IJsonApiClient<EventInstance, Guid> eventInstanceClient)
        {
            _cache = cache;
            _logger = logger;
            _jwtService = jwtService;
            _identityService = identityService;
            _eventInstanceClient = eventInstanceClient;
        }

        protected override async Task HandleRequirementAsync(
                        AuthorizationHandlerContext context,
                        AdminOrOwnsDraftOrderRequirement requirement,
                        DraftOrder resource)
        {
            _logger.LogInformation("Entering DraftOrderAuthorizationHandler");

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

                    return claimOrderId.Value == resource.Id.ToString();
                }))
                {
                    context.Succeed(requirement);
                    return;
                }
                // user is admin

            }
            else if (await _identityService.UserIsInRoleAsync("Admin", context.User))
            {
                _logger.LogInformation("PASSED as user admin");
                context.Succeed(requirement);
                return;
            }

            // user owns service
            var ei = await _eventInstanceClient.GetAsync(resource.EventInstanceId);
            if (context.User.Claims.Where(x => x.Type == "member-of-eoid").Any(x => x.Value == ei.EventOrganiserId.ToString()))
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}