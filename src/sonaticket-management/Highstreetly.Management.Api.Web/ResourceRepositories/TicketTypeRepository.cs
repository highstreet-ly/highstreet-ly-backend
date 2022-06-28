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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Web.ResourceRepositories
{
    public class TicketTypeRepository : EntityFrameworkCoreRepository<TicketType, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbContext _mgmtDbContext;

        public TicketTypeRepository(ITargetedFields targetedFields, IDbContextResolver contextResolver, IResourceGraph resourceGraph, IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _mgmtDbContext = contextResolver.GetContext();
            _httpContextAccessor = httpContextAccessor;
        }

        protected override IQueryable<TicketType> GetAll()
        {
            // admins see all
            if (_httpContextAccessor.IsAdmin())
            {
                return base.GetAll();
            }
            
            // event orgamisers see only theirs
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }
            
            var eoids = _httpContextAccessor
                       .HttpContext
                       .User
                       .FindAll("member-of-eoid")
                       .Select(x=> Guid.Parse(x.Value))
                       .ToList();

            if (eoids.Any())
            {
                var eventOrganiser = _mgmtDbContext
                                     .Set<EventOrganiser>()
                                     .Include(x => x.EventSeries)
                                     .ThenInclude(x => x.EventInstances)
                                     .Where(x =>  eoids.Contains(x.Id));

                var eids = eventOrganiser
                           .SelectMany(eo =>
                               eo.EventSeries
                                 .SelectMany(es =>
                                     es.EventInstances
                                       .Select(ei => ei.Id)))
                           .ToList();
                    
                return base.GetAll()
                           .Where(x => eids.Contains(x.EventInstanceId));
            }

            return default;
        }

        public override async Task UpdateAsync(TicketType resourceFromRequest, TicketType resourceFromDatabase, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var canWrite = await CanWriteAsync(resourceFromRequest, cancellationToken);

            if (!canWrite) throw new UnauthorizedAccessException();

            await  base.UpdateAsync(resourceFromRequest, resourceFromDatabase, cancellationToken);
        }

        public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }

            var resourceFromRequest = await _mgmtDbContext
                                            .Set<TicketType>()
                                            .FirstAsync(x => x.Id == id, cancellationToken: cancellationToken);
            
            var canWrite = await CanWriteAsync(resourceFromRequest, cancellationToken);

            if (!canWrite) throw new UnauthorizedAccessException();

            await  base.DeleteAsync(id, cancellationToken);
        }
        
        public override async Task CreateAsync(TicketType resourceFromRequest, TicketType resourceForDatabase, CancellationToken cancellationToken)
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
        
        private async Task<bool> CanWriteAsync(TicketType resourceFromRequest, CancellationToken cancellationToken)
        {
            var owningResource = await _mgmtDbContext
                                       .Set<EventInstance>()
                                       .FirstAsync(x => x.Id == resourceFromRequest.EventInstanceId, cancellationToken: cancellationToken);
            
            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(owningResource);
            return canWrite;
        }
    }
}