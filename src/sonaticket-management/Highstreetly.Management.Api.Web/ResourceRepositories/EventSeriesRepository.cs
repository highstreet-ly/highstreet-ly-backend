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
    public class EventSeriesRepository : EntityFrameworkCoreRepository<EventSeries, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbContext _managementDbContext;
        
        public EventSeriesRepository(ITargetedFields targetedFields, IDbContextResolver contextResolver, IResourceGraph resourceGraph, IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _managementDbContext = contextResolver.GetContext();
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var series = await _managementDbContext.Set<EventSeries>()
                                             .FirstAsync(x => x.Id == id, cancellationToken: cancellationToken);

            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(series.EventOrganiserId)
                           || _httpContextAccessor.OwnsResource(id);

            if (canWrite)
            {
                await base.DeleteAsync(id, cancellationToken);
                    return;
            }
            
            throw new UnauthorizedAccessException();
        }

        public override Task UpdateAsync(EventSeries resourceFromRequest, EventSeries resourceFromDatabase,
                                         CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(resourceFromRequest)
                           || _httpContextAccessor.OwnsResource(resourceFromRequest);

            return canWrite ? base.UpdateAsync(resourceFromRequest, resourceFromDatabase, cancellationToken) : throw new UnauthorizedAccessException();
        }

        public override Task CreateAsync(EventSeries resourceFromRequest, EventSeries resourceForDatabase, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(resourceFromRequest)
                           || _httpContextAccessor.OwnsResource(resourceFromRequest);

            return canWrite ? base.CreateAsync(resourceFromRequest, resourceForDatabase, cancellationToken) :  throw new UnauthorizedAccessException();
        }
    }
}