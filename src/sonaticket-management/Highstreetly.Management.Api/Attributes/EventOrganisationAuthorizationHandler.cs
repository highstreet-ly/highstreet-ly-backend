using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Management.Resources;
using Microsoft.AspNetCore.Authorization;

namespace Highstreetly.Management.Api.Attributes
{
    public class EventOrganisationAuthorizationHandler : AuthorizationHandler<AdminOrOwnsOrganisationRequirement, EventOrganiser>
    {
        private readonly IIdentityService _identityService;

        public EventOrganisationAuthorizationHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminOrOwnsOrganisationRequirement requirement,
            EventOrganiser resource)
        {
            // user owns resounce
            if (context.User.FindAll("member-of-eoid").Any(x=>x.Value == resource.Id.ToString()))
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
            // user is admin
            else
            {
                if (await _identityService.UserIsInRoleAsync("Admin", context.User))
                {
                    context.Succeed(requirement);
                    return;
                }
            }
        }
    }
    
}