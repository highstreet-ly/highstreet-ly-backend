using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Permissions.Resources;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Claim = System.Security.Claims.Claim;

namespace Highstreetly.Ids.Services
{
    public class ProfileService : IProfileService
    {
        readonly ILogger<ProfileService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly Permissions.PermissionsDbContext _idsIdsDbContext;
        private IJsonApiClient<Subscription, Guid> _subscriptionClient;

        public ProfileService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            Permissions.PermissionsDbContext idsIdsDbContext,
            ILogger<ProfileService> logger,
            IJsonApiClient<Subscription, Guid> subscriptionClient)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _idsIdsDbContext = idsIdsDbContext;
            _logger = logger;
            _subscriptionClient = subscriptionClient;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            _logger.LogInformation($"Entering ProfileService");
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.FirstOrDefault(x => x.Type == "sub").Value;

            var user = await _userManager.FindByIdAsync(subjectId);

            if (user == null)
                throw new ArgumentException("Invalid subject identifier");

            var claims = await GetClaimsFromUser(user);
            context.IssuedClaims = claims.ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.FirstOrDefault(x => x.Type == "sub").Value;
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                            return;
                    }
                }

                context.IsActive =
                                !user.LockoutEnabled ||
                                !user.LockoutEnd.HasValue ||
                                user.LockoutEnd <= DateTime.Now;
            }
        }

        private async Task<IEnumerable<Claim>> GetClaimsFromUser(User user)
        {
            _logger.LogInformation($"Entering GetClaimsFromUser");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                // new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            // we should support a single user belonging to multiple Event organisations
            var usersEventOrganiserClaim =
                            _idsIdsDbContext.UserClaims.FirstOrDefault(x =>
                                            x.ClaimType == "member-of-eoid" && x.UserId.ToString() == user.Id.ToString());

            if (usersEventOrganiserClaim != null)
            {
                _logger.LogInformation($"Adding eoid claim with value: { usersEventOrganiserClaim.ClaimValue}");
                claims.Add(new Claim("eoid", usersEventOrganiserClaim.ClaimValue));
                claims.Add(new Claim("member-of-eoid", usersEventOrganiserClaim.ClaimValue));
            }

            var usersStripeUserClaim =
                _idsIdsDbContext.UserClaims.FirstOrDefault(x =>
                    x.ClaimType == "stripe-customer-id" && x.UserId.ToString() == user.Id.ToString());

            if (usersStripeUserClaim != null)
            {
                claims.Add(new Claim("stripe-customer-id", usersStripeUserClaim.ClaimValue));
            }

            claims.Add(new Claim("role", "user"));

            foreach (var role in userRoles)
            {
                _logger.LogInformation($"Adding role claim with value {role}");
                claims.Add(new Claim("role", role));
            }

            if (!string.IsNullOrEmpty(user.FirstName))
            {
                claims.Add(new Claim("first_name", user.FirstName));
            }

            if (!string.IsNullOrEmpty(user.LastName))
            {
                claims.Add(new Claim("last_name", user.LastName));
            }

            var userClaims = await _userManager.GetClaimsAsync(user);

            claims.AddRange(userClaims);

            if (_userManager.SupportsUserEmail)
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("email_verified", user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                    //new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            var queryBuilder = new QueryBuilder()
                .Equalz("user-id", user.Id.ToString())
                .Includes(
                    "plan",
                    "add-ons",
                    "plan.features",
                    "add-ons.features");

            var subscriptions = await _subscriptionClient.GetListAsync(queryBuilder, allowApiAuthIfNeeded: true);

            var usersSubs = subscriptions.ToList();

            if (usersSubs.Any(x => x.CancelledAt == null))
            {
                var features = new List<string>();

                var sub = usersSubs.First(x => x.CancelledAt == null);

                foreach (var planFeature in sub.Plan.Features)
                {
                    if (features.All(x => x != planFeature.ClaimValue))
                    {
                        features.Add(planFeature.ClaimValue);
                    }
                }

                foreach (var addOnFeature in sub.AddOns.SelectMany(x => x.Features))
                {
                    if (features.All(x => x != addOnFeature.ClaimValue))
                    {
                        features.Add(addOnFeature.ClaimValue);
                    }
                }

                foreach (var feature in features)
                {
                    claims.Add(new Claim("feature", feature));
                }

            }

            return claims;
        }
    }
}

