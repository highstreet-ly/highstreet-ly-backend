using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Web.ResourceRepositories
{
    public class RoleRepository : EntityFrameworkCoreRepository<Role, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleRepository(
            ITargetedFields targetedFields,
            IDbContextResolver contextResolver,
            IResourceGraph resourceGraph,
            IResourceFactory resourceFactory,
            IEnumerable<IQueryConstraintProvider> constraintProviders,
            ILoggerFactory loggerFactory,
            IHttpContextAccessor httpContextAccessor) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override IQueryable<Role> GetAll()
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            if (isAdmin)
            {
                return base.GetAll();
            }

            return base.GetAll()
                .Where(x => x.Privileged == false);
        }

        public override Task DeleteAsync(
            Guid id,
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
            Role resourceFromRequest,
            Role resourceFromDatabase,
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
            Role resourceFromRequest,
            Role resourceForDatabase,
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