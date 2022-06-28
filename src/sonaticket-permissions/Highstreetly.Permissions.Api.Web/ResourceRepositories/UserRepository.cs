using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Web.ResourceRepositories
{
    public class UserRepository : EntityFrameworkCoreRepository<User, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;
        private readonly PermissionsDbContext _permissionsDbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            ITargetedFields targetedFields,
            IDbContextResolver contextResolver,
            IResourceGraph resourceGraph,
            IResourceFactory resourceFactory,
            IEnumerable<IQueryConstraintProvider> constraintProviders,
            IHttpContextAccessor httpContextAccessor,
            IJwtService jwtService,
            PermissionsDbContext permissionsDbContext,
            ILoggerFactory loggerFactory,
            ILogger<UserRepository> logger) : base(
            targetedFields,
            contextResolver,
            resourceGraph,
            resourceFactory,
            constraintProviders,
            loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
            _permissionsDbContext = permissionsDbContext;
            _logger = logger;
        }

        public override async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var resource = await _permissionsDbContext
                .Users
                .FirstAsync(
                x => x.Id == id,
                cancellationToken);

            var isAdmin = await CanWriteAsync(
                resource,
                cancellationToken);

            if (isAdmin)
            {
                await base.DeleteAsync(
                    id,
                    cancellationToken);
            }

            throw new UnauthorizedAccessException();
        }

        protected override IQueryable<User> GetAll()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return default;
            }

            // if admin they can see all
            if (_httpContextAccessor.IsAdmin())
            {
                return base.GetAll();
            }

            // in the same org:

            var eoid = _httpContextAccessor.HttpContext.User.FindFirstValue("eoid");

            var canParse = Guid.TryParse(eoid, out var requestingUsersEoid);

            if (canParse)
            {
                return base.GetAll()
                    .Where(x => x.CurrentEoid == requestingUsersEoid);
            }

            var subClaim = _httpContextAccessor.HttpContext.User.FindFirstValue("sub");

            if (!string.IsNullOrWhiteSpace(subClaim))
            {
                var canParseSub = Guid.TryParse(
                    subClaim,
                    out var sub);
                if (canParseSub)
                {
                    return base.GetAll().Where(x => x.Id ==sub);
                }
            }

            return base.GetAll().Where(x => x.Id == Guid.Empty);
        }

        private async Task<bool> CanWriteAsync(
            User resourceFromRequest,
            CancellationToken cancellationToken)
        {
            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OwnsResource(resourceFromRequest.Id);
            if (canWrite)
            {
                return true;
            }

            var claimOrderId = await GetClaimOrderId();
            return claimOrderId == resourceFromRequest.Id;
        }

        private async Task<Guid> GetClaimOrderId()
        {
            var token = _jwtService.GetAccessToken();
            var orderId = Guid.Empty;

            if (!string.IsNullOrEmpty(token))
            {
                await _jwtService.ValidateTokenAsync(
                    token,
                    claimsPrincipal =>
                    {
                        var claimOrderId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "order-id");
                        if (claimOrderId == null)
                        {
                            return false;
                        }

                        var canParse = Guid.TryParse(claimOrderId.Value, out var oid);
                        if (canParse)
                        {
                            orderId = oid;
                        }
                        return canParse;
                    });
            }

            return orderId;
        }
    }
}