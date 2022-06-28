using System;
using System.Collections.Generic;
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
    public class BusinessTypeFeatureTemplateRepository : EntityFrameworkCoreRepository<BusinessTypeFeatureTemplate, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BusinessTypeFeatureTemplateRepository(ITargetedFields targetedFields, IDbContextResolver contextResolver, IResourceGraph resourceGraph, IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task UpdateAsync(BusinessTypeFeatureTemplate resourceFromRequest, BusinessTypeFeatureTemplate resourceFromDatabase,
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

        public override Task CreateAsync(BusinessTypeFeatureTemplate resourceFromRequest, BusinessTypeFeatureTemplate resourceForDatabase, CancellationToken cancellationToken)
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