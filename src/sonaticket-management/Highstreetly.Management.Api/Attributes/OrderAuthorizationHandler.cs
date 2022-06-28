using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Identity;
using Highstreetly.Management.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Attributes
{
    public class OrderAuthorizationHandler : AuthorizationHandler<AdminOrOwnsOrderRequirement, Order>
    {
        private readonly IJwtService _jwtService;
        private readonly IIdentityService _identityService;
        private readonly ILogger<Order> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public OrderAuthorizationHandler(
                        IJwtService jwtService,
                        IIdentityService identityService,
                        ILogger<Order> logger,
                        ManagementDbContext managementDbContext)
        {
            _jwtService = jwtService;
            _identityService = identityService;
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                        AdminOrOwnsOrderRequirement requirement,
                        Order resource)
        {
            _logger.LogInformation("Entering OrderAuthorizationHandler");

            var token = _jwtService.GetAccessToken();

            // user owns resource
            if (resource.OwnerId != null && resource.OwnerId != Guid.Empty && context.User.FindFirstValue("sub") == resource.OwnerId.ToString())
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
                _logger.LogInformation("we have a token");
                if (await _jwtService.ValidateTokenAsync(token, (claimsPrincipal) =>
                {
                    var claimOrderId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "order-id");
                    if (claimOrderId == null)
                    {
                        _logger.LogInformation("we have a token");
                        return false;
                    }

                    return claimOrderId.Value == resource.Id.ToString();
                }))
                {
                    context.Succeed(requirement);
                    return;
                }

            }// user is admin
            else if (await _identityService.UserIsInRoleAsync("Admin", context.User))
            {
                _logger.LogInformation("PASSED as user admin");
                context.Succeed(requirement);
                return;
            }

            // user belongs to org that owns the service
            var ei = await _managementDbContext.EventInstances.FindAsync(resource.EventInstanceId);
            if (ei != null && context.User.FindAll("member-of-eoid").Any(x=>x.Value ==  ei.EventOrganiserId.ToString()))
            {
                _logger.LogInformation(
                                $"PASSED on eoid with ownerid: {resource.OwnerId} and eoid: {context.User.FindFirstValue("eoid")}");
                context.Succeed(requirement);
                return;

            }

            // is the user an admin?
            if (await _identityService.UserIsInRoleAsync("Admin", context.User))
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}