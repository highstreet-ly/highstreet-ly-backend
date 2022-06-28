using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Web.ResourceRepositories
{
    public class ClaimRepository : EntityFrameworkCoreRepository<Claim, int>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimRepository(
            ITargetedFields targetedFields,
            IDbContextResolver contextResolver,
            IResourceGraph resourceGraph,
            IResourceFactory resourceFactory,
            IEnumerable<IQueryConstraintProvider> constraintProviders,
            ILoggerFactory loggerFactory,
            IHttpContextAccessor httpContextAccessor) : base(targetedFields,
            contextResolver,
            resourceGraph,
            resourceFactory,
            constraintProviders,
            loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task DeleteAsync(
            int id,
            CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            if (isAdmin)
            {
                return base.DeleteAsync(
                    id,
                    cancellationToken);
            }

            throw new UnauthorizedAccessException();
        }

        public override Task UpdateAsync(
            Claim resourceFromRequest,
            Claim resourceFromDatabase,
            CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            if (isAdmin)
            {
                return base.UpdateAsync(
                    resourceFromRequest,
                    resourceFromDatabase,
                    cancellationToken);
            }

            throw new UnauthorizedAccessException();
        }

        public override Task CreateAsync(
            Claim resourceFromRequest,
            Claim resourceForDatabase,
            CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            if (isAdmin)
            {
                return base.CreateAsync(
                    resourceFromRequest,
                    resourceForDatabase,
                    cancellationToken);
            }

            throw new UnauthorizedAccessException();
        }
    }
}