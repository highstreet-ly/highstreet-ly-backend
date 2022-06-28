using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
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
    public class OrderRepository : EntityFrameworkCoreRepository<Order, Guid>
    {
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ILogger<OrderRepository> _logger;
        private readonly DbContext _mgmtDbContext;
        
        public OrderRepository(
            ITargetedFields targetedFields, 
            IDbContextResolver contextResolver, 
            IResourceGraph resourceGraph, 
            IResourceFactory resourceFactory, 
            IEnumerable<IQueryConstraintProvider> constraintProviders, 
            ILoggerFactory loggerFactory,
            IJwtService jwtService, 
            IHttpContextAccessor httpContextAccessor) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _mgmtDbContext = contextResolver.GetContext();
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger<OrderRepository>();
        }

        public override async Task UpdateAsync(Order resourceFromRequest, Order resourceFromDatabase, CancellationToken cancellationToken)
        {
            var canWrite = await CanWriteAsync(resourceFromRequest, cancellationToken);

            if (!canWrite) throw new UnauthorizedAccessException();
            
            await base.UpdateAsync(resourceFromRequest, resourceFromDatabase, cancellationToken);
        }

        public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var resourceFromRequest =await _mgmtDbContext.Set<Order>()
                                                    .FirstAsync(x => x.Id == id, cancellationToken: cancellationToken);
            var canWrite = await CanWriteAsync(resourceFromRequest, cancellationToken);

            if (!canWrite) throw new UnauthorizedAccessException();
            
            await base.DeleteAsync(id, cancellationToken);
        }

        protected override IQueryable<Order> GetAll()
        {
            var claimOrderId =  GetClaimOrderId().GetAwaiter().GetResult();

            _logger.LogInformation("Entering IQueryable<Order> GetAll");

            // if organiser (not admin and no order-id in the jwt) they can see all orders
            // belonging to all of their event instances
            if (!_httpContextAccessor.IsAdmin() && claimOrderId == Guid.Empty)
            {
                _logger.LogInformation("Entering IQueryable<Order> GetAll:: NOT ADMIN");

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

                _logger.LogInformation($"Entering IQueryable<Order> GetAll:: eoids: {string.Join(",", eoids)}");

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

                    _logger.LogInformation($"Entering IQueryable<Order> GetAll:: eids: {string.Join(",", eids)}");

                    return base.GetAll()
                               .Where(x => eids.Contains(x.EventInstanceId));
                }
            }

            // if admin they can see all
            if (_httpContextAccessor.IsAdmin())
            {
                return base.GetAll();
            }
            
            // if none of the above they can only see the one in the jwt
            return base.GetAll().Where(x=>x.Id == claimOrderId);
        }

        private async Task<bool> CanWriteAsync(Order resourceFromRequest, CancellationToken cancellationToken)
        {
            var owningResource = await _mgmtDbContext
                                       .Set<EventInstance>()
                                       .FirstAsync(x => x.Id == resourceFromRequest.EventInstanceId, cancellationToken: cancellationToken);
            
            var canWrite = _httpContextAccessor.IsAdmin()
                           || _httpContextAccessor.OrganisesResource(owningResource);
            if (canWrite)
            {
                return true;
            }

            var claimOrderId = await GetClaimOrderId();
            return claimOrderId == resourceFromRequest.Id;
        }

        private async Task<Guid> GetClaimOrderId()
        {
            var token = _jwtService.GetAccessToken();
            var orderId = Guid.Empty;

            if (!string.IsNullOrEmpty(token))
            {
                await _jwtService.ValidateTokenAsync(token, (claimsPrincipal) =>
                {
                    var claimOrderId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "order-id");
                    if (claimOrderId == null)
                    {
                        return false;
                    }

                    orderId = Guid.Parse(claimOrderId.Value);
                    return true;
                });
            }

            return orderId;
        }
    }
}