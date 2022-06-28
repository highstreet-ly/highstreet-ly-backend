using Microsoft.AspNetCore.Authorization;

namespace Highstreetly.Management.Api.Attributes
{
    public class AdminOrOwnsEventRequirement : IAuthorizationRequirement
    {
    }
}