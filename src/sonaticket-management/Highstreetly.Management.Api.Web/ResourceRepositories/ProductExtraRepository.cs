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
    public class ProductExtraRepository : EntityFrameworkCoreRepository<ProductExtra, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbContext _mgmtDbContext;

        public ProductExtraRepository(ITargetedFields targetedFields, IDbContextResolver contextResolver, IResourceGraph resourceGraph, IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _mgmtDbContext = contextResolver.GetContext();
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task UpdateAsync(ProductExtra resourceFromRequest, ProductExtra resourceFromDatabase, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = await CanWriteAsync(resourceFromRequest, cancellationToken);

            if (!canWrite) throw new UnauthorizedAccessException();

            await base.UpdateAsync(resourceFromRequest, resourceFromDatabase, cancellationToken) ;
        }

        public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }
            
            var resourceFromRequest = await _mgmtDbContext
                                            .Set<ProductExtra>()
                                            .FirstAsync(x => x.Id == id, cancellationToken: cancellationToken);
            
            var canWrite = await CanWriteAsync(resourceFromRequest, cancellationToken);

            if (!canWrite) throw new UnauthorizedAccessException();

            await  base.DeleteAsync(id, cancellationToken);
        }
        
        public override async Task CreateAsync(ProductExtra resourceFromRequest, ProductExtra resourceForDatabase, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = await CanWriteAsync(resourceFromRequest, cancellationToken);

            if (!canWrite) throw new UnauthorizedAccessException();

            await base.CreateAsync(resourceFromRequest, resourceForDatabase, cancellationToken) ;
        }
        
        private async Task<bool> CanWriteAsync(ProductExtra resourceFromRequest, CancellationToken cancellationToken)
        {
            var ttId = await GetTicketTypeId(resourceFromRequest);

            var tt = await _mgmtDbContext
                .Set<TicketTypeConfiguration>()
                .FirstAsync(
                    x => x.Id == ttId, cancellationToken:
                    cancellationToken);

            var owningResource = await _mgmtDbContext
                                       .Set<EventInstance>()
                                       .FirstAsync(x => x.Id == tt.EventInstanceId, cancellationToken: cancellationToken);
            
            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(owningResource);
            return canWrite;
        }

        private async Task<Guid> GetTicketTypeId(ProductExtra resourceFromRequest)
        {
            var result = Guid.Empty;

            if (resourceFromRequest.TicketTypeId.HasValue && resourceFromRequest.TicketTypeId != Guid.Empty)
            {
                result = resourceFromRequest.TicketTypeId.Value;
            }
            else if (resourceFromRequest.TicketType != null && resourceFromRequest.TicketType.Id != Guid.Empty)
            {
                result = resourceFromRequest.TicketType.Id;
            }
            else if (resourceFromRequest.ProductExtraGroupId.HasValue && resourceFromRequest.ProductExtraGroupId.Value != Guid.Empty)
            {
                var tt = await _mgmtDbContext.Set<ProductExtraGroup>()
                    .Include(x => x.TicketTypeConfiguration)
                    .FirstAsync(x => x.Id == resourceFromRequest.ProductExtraGroupId);
                result = tt.TicketTypeConfiguration.Id;
            }
            else if (resourceFromRequest.ProductExtraGroup != null && resourceFromRequest.ProductExtraGroup.Id != Guid.Empty)
            {
                var tt = await _mgmtDbContext.Set<ProductExtraGroup>()
                    .Include(x => x.TicketTypeConfiguration)
                    .FirstAsync(x => x.Id == resourceFromRequest.ProductExtraGroup.Id);
                result = tt.TicketTypeConfiguration.Id;
            }

            return result;
        }
    }
}