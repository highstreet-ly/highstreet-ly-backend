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
    public class EventOrganiserRepository : EntityFrameworkCoreRepository<EventOrganiser, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventOrganiserRepository(ITargetedFields targetedFields, IDbContextResolver contextResolver, IResourceGraph resourceGraph, IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(id)
                           || _httpContextAccessor.OwnsResource(id);

            return canWrite ? base.DeleteAsync(id, cancellationToken) : throw new UnauthorizedAccessException();
        }

        public override Task UpdateAsync(EventOrganiser resourceFromRequest,
                                         EventOrganiser resourceFromDatabase,
                                         CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(resourceFromRequest.Id)
                           || _httpContextAccessor.OwnsResource(resourceFromRequest.Id);

            return canWrite ? base.UpdateAsync(resourceFromRequest, resourceFromDatabase, cancellationToken) : throw new UnauthorizedAccessException();
        }

        public override Task CreateAsync(EventOrganiser resourceFromRequest, EventOrganiser resourceForDatabase, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(resourceFromRequest.Id)
                           || _httpContextAccessor.OwnsResource(resourceFromRequest.Id);

            return canWrite ? base.CreateAsync(resourceFromRequest, resourceForDatabase, cancellationToken) : throw new UnauthorizedAccessException();
        }
    }
}