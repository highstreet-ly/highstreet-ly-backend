using Microsoft.AspNetCore.Authorization;

namespace Highstreetly.Reservations.Api.Attributes
{
    public class AdminOrOwnsDraftOrderRequirement : IAuthorizationRequirement { }
}