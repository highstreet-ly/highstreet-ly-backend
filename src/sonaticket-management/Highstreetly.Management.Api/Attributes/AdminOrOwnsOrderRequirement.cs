using Microsoft.AspNetCore.Authorization;

namespace Highstreetly.Management.Api.Attributes
{
    public class AdminOrOwnsOrderRequirement : IAuthorizationRequirement
    {
    }
}