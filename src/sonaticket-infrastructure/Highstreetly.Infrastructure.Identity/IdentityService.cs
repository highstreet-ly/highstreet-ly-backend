using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Permissions.Contracts.Requests;
using JsonApiSerializer.JsonApi;
using Microsoft.Extensions.Caching.Memory;
using Claim = Highstreetly.Permissions.Contracts.Requests.Claim;

namespace Highstreetly.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IMemoryCache _cache;
        private readonly IJsonApiClient<User, Guid> _userClient;
        private readonly IJsonApiClient<Role, Guid> _roleClient;
        private readonly IJsonApiClient<Claim, Guid> _claimClient;

        public IdentityService(
            IMemoryCache cache,
            IJsonApiClient<User, Guid> userClient,
            IJsonApiClient<Role, Guid> roleClient,
            IJsonApiClient<Claim, Guid> claimClient)
        {
            _cache = cache;
            _userClient = userClient;
            _roleClient = roleClient;
            _claimClient = claimClient;
        }

        public async Task SetUserCurrentEoid(User user, Guid eoid)
        {
            var u = await _userClient.GetAsync(user.Id, new QueryBuilder().Includes(includes: new[] { "claims", "roles" }));
            u.CurrentEoid = eoid;
            await _userClient.UpdateAsync(u.Id, u);
        }

        public async Task AddUserToRoleAsync(User user, Role role)
        {
            var u = await _userClient.GetAsync(user.Id, new QueryBuilder().Includes(includes: new[] { "claims", "roles" }));
            u.Roles.Add(role);
            await _userClient.UpdateAsync(u.Id, u);
        }

        public async Task AddUserToRolesAsync(User user, List<Role> roles)
        {
            foreach (var role in roles)
            {
                await AddUserToRoleAsync(user, role);
            }
        }

        public async Task AddUserClaimAsync(User user, Claim claim)
        {
            var u = await _userClient.GetAsync(user.Id, new QueryBuilder().Includes(includes: new[] { "claims", "roles" }));

            claim.User = u;

            var c = await _claimClient.CreateAsync(claim);

            u.Claims.Add(c);
            await _userClient.UpdateAsync(user.Id, u);
        }

        public async Task<bool> UserIsInRoleAsync(string role, ClaimsPrincipal subject)
        {
            //var subject = _context.HttpContext.User;

            var subjectId = subject.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            if (subjectId != null)
            {
                var roles = await GetUserRolesAsync(subjectId);

                return roles.Any(x => string.Equals(x.Name, role, StringComparison.CurrentCultureIgnoreCase));
            }

            return default;
        }

        public async Task<List<Role>> GetUserRolesAsync(string sub)
        {
            if (string.IsNullOrEmpty(sub))
            {
                return new List<Role>();
            }

            var key = "IdentityService_Roles_" + sub;

            if (_cache.Get(key) is not List<Role> roles)
            {
                var user = await _userClient.GetAsync(Guid.Parse(sub), new QueryBuilder().Includes(includes: new[] { "roles" }));
                roles = user.Roles;
                _cache.Set(key, roles, DateTime.UtcNow.AddMinutes(1));
            }

            return roles;
        }

        public Task<User> GetUser(string sub)
        {
            return _userClient.GetAsync(Guid.Parse(sub), new QueryBuilder().Includes(includes: new[] { "claims", "roles" }));
        }

        public async Task<IEnumerable<Role>> GetRole(string role)
        {
            var queryBuilder = new QueryBuilder().Equalz("name", role);

            return await _roleClient.GetListAsync(queryBuilder);
        }

        public async Task<List<Claim>> GetUserClaimsAsync(string sub)
        {
            if (string.IsNullOrEmpty(sub))
            {
                return new List<Claim>();
            }

            var key = "IdentityService_Claims_" + sub;

            if (!(_cache.Get(key) is IList<Claim> cachedClaims))
            {
                var user = await _userClient.GetAsync(Guid.Parse(sub), new QueryBuilder().Includes(includes: new[] { "claims" }));
                cachedClaims = user.Claims;
                _cache.Set(key, cachedClaims, DateTime.UtcNow.AddMinutes(1));
            }

            return cachedClaims.ToList();
        }
    }
}