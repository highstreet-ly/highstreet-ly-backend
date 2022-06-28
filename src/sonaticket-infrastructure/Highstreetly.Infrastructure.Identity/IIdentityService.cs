using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Highstreetly.Permissions.Contracts.Requests;
using Claim = Highstreetly.Permissions.Contracts.Requests.Claim;

namespace Highstreetly.Infrastructure.Identity
{
    public interface IIdentityService
    {
        Task<bool> UserIsInRoleAsync(string role, ClaimsPrincipal subject);
        Task<List<Role>> GetUserRolesAsync(string sub);
        Task<List<Claim>> GetUserClaimsAsync(string sub);
        Task AddUserToRoleAsync(User user, Role role);
        Task AddUserToRolesAsync(User user, List<Role> roles);
        Task AddUserClaimAsync(User user, Claim claim);
        Task<User> GetUser(string sub);
        Task<IEnumerable<Role>> GetRole(string role);
        Task SetUserCurrentEoid(User user, Guid eoid);
    }
}