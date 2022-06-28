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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Web.ResourceRepositories
{
    public class ProductCategoryRepository : EntityFrameworkCoreRepository<ProductCategory, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbContext _mgmtDbContext;

        public ProductCategoryRepository(
            ITargetedFields targetedFields, 
            IDbContextResolver contextResolver,
            IResourceGraph resourceGraph,
            IResourceFactory resourceFactory, 
            IEnumerable<IQueryConstraintProvider> constraintProviders, 
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory) 
            : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _mgmtDbContext = contextResolver.GetContext();
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task UpdateAsync(ProductCategory resourceFromRequest,
                                               ProductCategory resourceFromDatabase,
                                               CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = await CanWriteAsync(resourceFromRequest, cancellationToken);

            if (!canWrite) throw new UnauthorizedAccessException();
            
            await base.UpdateAsync(resourceFromRequest, resourceFromDatabase, cancellationToken);
        }

        private async Task<bool> CanWriteAsync(ProductCategory resourceFromRequest, CancellationToken cancellationToken)
        {
            var owningResource = await _mgmtDbContext
                                       .Set<EventInstance>()
                                       .FirstAsync(
                                           x => x.Id == resourceFromRequest.EventInstanceId, cancellationToken:
                                           cancellationToken);

            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(owningResource);
            return canWrite;
        }

        public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var productCategory = await _mgmtDbContext
                                       .Set<ProductCategory>()
                                       .FirstAsync(
                                           x => x.Id == id, cancellationToken: cancellationToken);

            var canWrite = await CanWriteAsync(productCategory, cancellationToken);

            if (!canWrite) throw new UnauthorizedAccessException();

            await base.DeleteAsync(id, cancellationToken);
        }

        public override async Task CreateAsync(ProductCategory resourceFromRequest, ProductCategory resourceForDatabase,
                                               CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = await CanWriteAsync(resourceFromRequest, cancellationToken);

            if (!canWrite) throw new UnauthorizedAccessException();

            await base.CreateAsync(resourceFromRequest, resourceForDatabase, cancellationToken);
        }
    }
}