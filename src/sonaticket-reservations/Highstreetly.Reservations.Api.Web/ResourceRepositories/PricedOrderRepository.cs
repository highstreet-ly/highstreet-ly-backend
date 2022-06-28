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
    public class PricedOrderRepository : EntityFrameworkCoreRepository<PricedOrder, Guid>
    {
        private readonly IJsonApiClient<EventInstance, Guid> _eventInstanceApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;
        private readonly ReservationDbContext _reservationDbContext;

        public PricedOrderRepository(
            ITargetedFields targetedFields,
            IDbContextResolver contextResolver,
            IResourceGraph resourceGraph,
            IResourceFactory resourceFactory,
            IEnumerable<IQueryConstraintProvider> constraintProviders,
            IJsonApiClient<EventInstance, Guid> eventInstanceApiClient,
            IHttpContextAccessor httpContextAccessor,
            IJwtService jwtService,
            ReservationDbContext reservationDbContext,
            ILoggerFactory loggerFactory) : base(targetedFields,
            contextResolver,
            resourceGraph,
            resourceFactory,
            constraintProviders,
            loggerFactory)
        {
            _eventInstanceApiClient = eventInstanceApiClient;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
            _reservationDbContext = reservationDbContext;
        }

        public override async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var resourceFromRequest = await _reservationDbContext.Set<PricedOrder>()
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

        protected override IQueryable<PricedOrder> GetAll()
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
                        .Where(x => eventInstanceIds.Contains(x.EventInstanceId.GetValueOrDefault()));
                }
            }

            return base.GetAll()
                .Where(x => x.OrderId == claimOrderId);
        }

        public override async Task CreateAsync(
            PricedOrder resourceFromRequest,
            PricedOrder resourceForDatabase,
            CancellationToken cancellationToken)
        {
            var canWrite = await CanWriteAsync(
                resourceFromRequest,
                cancellationToken);

            if (!canWrite)
            {
                throw new UnauthorizedAccessException();
            }

            await base.CreateAsync(
                resourceFromRequest,
                resourceForDatabase,
                cancellationToken);
        }

        public override async Task UpdateAsync(
            PricedOrder resourceFromRequest,
            PricedOrder resourceFromDatabase,
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
            PricedOrder resourceFromRequest,
            CancellationToken cancellationToken)
        {
            var canWrite = _httpContextAccessor.IsAdmin()
                            || _httpContextAccessor.OwnsResource((IHasOwner)resourceFromRequest);
            if (canWrite)
            {
                return true;
            }

            var claimOrderId = await GetClaimOrderId();
            return claimOrderId == resourceFromRequest.OrderId;
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