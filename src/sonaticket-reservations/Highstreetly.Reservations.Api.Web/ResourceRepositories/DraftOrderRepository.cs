using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Reservations.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Api.Web.ResourceRepositories
{
    public class DraftOrderRepository : EntityFrameworkCoreRepository<DraftOrder, Guid>
    {
        private readonly IJsonApiClient<EventInstance, Guid> _eventInstanceApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;
        private readonly ReservationDbContext _reservationDbContext;

        public DraftOrderRepository(
            ITargetedFields targetedFields,
            IDbContextResolver contextResolver,
            IResourceGraph resourceGraph,
            IResourceFactory resourceFactory,
            IEnumerable<IQueryConstraintProvider> constraintProviders,
            IHttpContextAccessor httpContextAccessor,
            IJwtService jwtService,
            ReservationDbContext reservationDbContext,
            IJsonApiClient<EventInstance, Guid> eventInstanceApiClient,
            ILoggerFactory loggerFactory) : base(
            targetedFields,
            contextResolver,
            resourceGraph,
            resourceFactory,
            constraintProviders,
            loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
            _reservationDbContext = reservationDbContext;
            _eventInstanceApiClient = eventInstanceApiClient;
        }

        public override async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var resourceFromRequest = await _reservationDbContext.Set<DraftOrder>()
                .FirstAsync(
                    x => x.Id == id,
                    cancellationToken);

            var canWrite = await CanWriteAsync(
                resourceFromRequest,
                cancellationToken);

            if (!canWrite)
            {
                throw new UnauthorizedAccessException();
            }

            await base.DeleteAsync(
                id,
                cancellationToken);
        }

        protected override IQueryable<DraftOrder> GetAll()
        {
            // if admin they can see all
            if (_httpContextAccessor.IsAdmin())
            {
                return base.GetAll();
            }

            var claimOrderId = GetClaimOrderId()
                .GetAwaiter()
                .GetResult();

            // if organiser (not admin and no order-id in the jwt) they can see all orders belonging to all of their event instances
            if (!_httpContextAccessor.IsAdmin() && claimOrderId == Guid.Empty)
            {
                if (_httpContextAccessor
                    .HttpContext == null)
                {
                    throw new UnauthorizedAccessException();
                }

                var eoids = _httpContextAccessor
                    .HttpContext
                    .User
                    .FindAll("member-of-eoid")
                    .Select(x => Guid.Parse(x.Value))
                    .ToList();

                if (eoids.Any())
                {
                    // get all orders where the eid is in the org
                    var eventInstanceIds = _eventInstanceApiClient
                        .GetEventInstanceIdsForOrganiserId(_httpContextAccessor)
                        .GetAwaiter()
                        .GetResult();

                    return base.GetAll()
                        .Where(x => eventInstanceIds.Contains(x.EventInstanceId));
                }
            }

            if (claimOrderId != Guid.Empty)
            {
                return base.GetAll()
                    .Where(x => x.Id == claimOrderId);
            }

            // this is clearly not right but I am not sure what to do here since JADNC complains if i limit this
            // TODO: fix this!!! 
            // TOTEST: create a POST to the draft-orders endpoint and it will blow up unless this is there :/
            return base.GetAll();
        }

        public override async Task UpdateAsync(
            DraftOrder resourceFromRequest,
            DraftOrder resourceFromDatabase,
            CancellationToken cancellationToken)
        {
            var canWrite = await CanWriteAsync(
                resourceFromRequest,
                cancellationToken);

            if (!canWrite)
            {
                throw new UnauthorizedAccessException();
            }

            await base.UpdateAsync(
                resourceFromRequest,
                resourceFromDatabase,
                cancellationToken);
        }

        private async Task<bool> CanWriteAsync(
            DraftOrder resourceFromRequest,
            CancellationToken cancellationToken)
        {
            var owningResource = await _reservationDbContext
                .Set<DraftOrder>()
                .FirstOrDefaultAsync(
                    x => x.Id == resourceFromRequest.Id,
                    cancellationToken);

            if (owningResource != null)
            {
                var canWrite = _httpContextAccessor.IsAdmin()
                               || _httpContextAccessor.OwnsResource((IHasOwner) owningResource);
                if (canWrite)
                {
                    return true;
                }

                var claimOrderId = await GetClaimOrderId();
                return claimOrderId == resourceFromRequest.Id;
            }

            // means we are creating fresh
            return true;
        }

        private async Task<Guid> GetClaimOrderId()
        {
            var token = _jwtService.GetAccessToken();
            var orderId = Guid.Empty;

            if (!string.IsNullOrEmpty(token))
            {
                await _jwtService.ValidateTokenAsync(
                    token,
                    claimsPrincipal =>
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