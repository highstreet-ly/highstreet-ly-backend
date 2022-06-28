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
    public class ProductExtraGroupRepository : EntityFrameworkCoreRepository<ProductExtraGroup, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbContext _mgmtDbContext;

        public ProductExtraGroupRepository(ITargetedFields targetedFields, IDbContextResolver contextResolver, IResourceGraph resourceGraph, IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _mgmtDbContext = contextResolver.GetContext();
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task UpdateAsync(ProductExtraGroup resourceFromRequest, ProductExtraGroup resourceFromDatabase, CancellationToken cancellationToken)
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

        public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var resourceFromRequest = await _mgmtDbContext
                                            .Set<ProductExtraGroup>()
                                            .FirstAsync(x => x.Id == id, cancellationToken: cancellationToken);

            var canWrite = await CanWriteAsync(resourceFromRequest, cancellationToken);

            if (!canWrite) throw new UnauthorizedAccessException();

            await base.DeleteAsync(id, cancellationToken);
        }

        public override async Task CreateAsync(ProductExtraGroup resourceFromRequest, ProductExtraGroup resourceForDatabase, CancellationToken cancellationToken)
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

        private async Task<bool> CanWriteAsync(ProductExtraGroup resourceFromRequest, CancellationToken cancellationToken)
        {
            var tt = await _mgmtDbContext
                                       .Set<TicketTypeConfiguration>()
                                       .FirstAsync(
                                           x => x.Id == GetTicketTypeId(resourceFromRequest), cancellationToken:
                                           cancellationToken);

            var owningResource = await _mgmtDbContext
                                   .Set<EventInstance>()
                                   .FirstAsync(x => x.Id == tt.EventInstanceId, cancellationToken: cancellationToken);

            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(owningResource);
            return canWrite;
        }

        private Guid GetTicketTypeId(ProductExtraGroup resourceFromRequest)
        {
            var result = Guid.Empty;

            if (resourceFromRequest.TicketTypeConfigurationId.HasValue &&  resourceFromRequest.TicketTypeConfigurationId != Guid.Empty)
            {
                result = resourceFromRequest.TicketTypeConfigurationId.Value;
            }
            else if (resourceFromRequest.TicketTypeId.HasValue && resourceFromRequest.TicketTypeId != Guid.Empty)
            {
                result = resourceFromRequest.TicketTypeId.Value;
            }
            else if (resourceFromRequest.TicketTypeConfiguration != null && resourceFromRequest.TicketTypeConfiguration.Id != Guid.Empty)
            {
                result = resourceFromRequest.TicketTypeConfiguration.Id;
            }
            else if (resourceFromRequest.TicketType != null && resourceFromRequest.TicketType.Id != Guid.Empty)
            {
                result = resourceFromRequest.TicketType.Id;
            }

            return result;
        }
    }
}