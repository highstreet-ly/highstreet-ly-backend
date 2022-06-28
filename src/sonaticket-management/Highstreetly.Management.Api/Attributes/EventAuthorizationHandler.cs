using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Management.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Attributes
{
    public class EventAuthorizationHandler : AuthorizationHandler<AdminOrOwnsEventRequirement, EventInstance>
    {
        private readonly ILogger<EventAuthorizationHandler> _logger;
        private readonly IIdentityService _identityService;

        public EventAuthorizationHandler(ILogger<EventAuthorizationHandler> logger, IIdentityService identityService)
        {
            _logger = logger;
            _identityService = identityService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminOrOwnsEventRequirement requirement,
            EventInstance resource)
        {
            _logger.LogInformation($"Checking user auth for event instance {resource.Name} for user {context.User.FindFirstValue("sub")}");
            // user owns resource
            if (!string.IsNullOrEmpty(resource.OwnerId.ToString()) && context.User.FindFirstValue("sub") == resource.OwnerId.ToString())
            {
                _logger.LogInformation($"PASSED on sub with ownerid: {resource.OwnerId} and sub: {context.User.FindFirstValue("sub")}");
                context.Succeed(requirement);
                return;
            }

            // user belongs to org that owns the event
            if (context.User.FindAll("member-of-eoid").Any(x=>x.Value == resource.EventOrganiserId.ToString()))
            {
                _logger.LogInformation($"PASSED on member-of-eoid with ownerid: {resource.OwnerId} and member-of-eoid: {string.Join(",", context.User.FindAll("eoid").Select(x=>x.Value).ToArray())}");
                context.Succeed(requirement);
                return;
            }

            // user is a backend service
            else if (context.User.FindFirstValue("access-all") != null)
            {
                _logger.LogInformation("PASSED as user is a backend service with the correct scope");
                context.Succeed(requirement);
                return;
            }
            // user is admin
            else
            {
                if (await _identityService.UserIsInRoleAsync("Admin", context.User))
                {
                    _logger.LogInformation("PASSED as user admin");
                    context.Succeed(requirement);
                    return;
                }
            }
        }
    }
}