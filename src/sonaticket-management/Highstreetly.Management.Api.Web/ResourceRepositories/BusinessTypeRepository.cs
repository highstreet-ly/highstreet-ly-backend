using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Web.ResourceRepositories
{
    public class BusinessTypeRepository : EntityFrameworkCoreRepository<BusinessType, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BusinessTypeRepository(ITargetedFields targetedFields, IDbContextResolver contextResolver, IResourceGraph resourceGraph, IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override IQueryable<BusinessType> GetAll()
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!_httpContextAccessor.IsAdmin())
            {
                return base.GetAll()
                    .Where(x => x.IsPublished);
            }

            return base.GetAll();
        }

        public override Task UpdateAsync(BusinessType resourceFromRequest, BusinessType resourceFromDatabase,
                                         CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                  throw new UnauthorizedAccessException();
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            return isAdmin ? base.UpdateAsync(resourceFromRequest, resourceFromDatabase, cancellationToken) :  throw new UnauthorizedAccessException();
        }

        public override Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            return isAdmin ? base.DeleteAsync(id, cancellationToken) :  throw new UnauthorizedAccessException();
        }

        public override Task CreateAsync(BusinessType resourceFromRequest, BusinessType resourceForDatabase, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            return isAdmin ? base.CreateAsync(resourceFromRequest, resourceForDatabase, cancellationToken) :  throw new UnauthorizedAccessException();
        }
    }
}