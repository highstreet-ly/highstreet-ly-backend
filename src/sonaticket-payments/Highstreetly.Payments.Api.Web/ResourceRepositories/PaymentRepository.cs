using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Payments.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Queries.Expressions;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.Api.Web.ResourceRepositories
{
    public class PaymentRepository : EntityFrameworkCoreRepository<Payment, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJsonApiClient<EventInstance, Guid> _eventInstanceClient;
        
        public PaymentRepository(
            ITargetedFields targetedFields, 
            IDbContextResolver contextResolver, 
            IResourceGraph resourceGraph, 
            IResourceFactory resourceFactory, 
            IEnumerable<IQueryConstraintProvider> constraintProviders,
            ILoggerFactory loggerFactory, 
            IHttpContextAccessor httpContextAccessor, IJsonApiClient<EventInstance, Guid> eventInstanceClient) : base(targetedFields, contextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _eventInstanceClient = eventInstanceClient;
        }


        public override async Task<int> CountAsync(FilterExpression topFilter, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                return 0;
            }

            var isAdmin = _httpContextAccessor.IsAdmin();

            if (isAdmin)
            {
                return await base.CountAsync(topFilter, cancellationToken);
            }

            var eventInstanceIds = await _eventInstanceClient.GetEventInstanceIdsForOrganiserId(_httpContextAccessor);

            if (eventInstanceIds != null && eventInstanceIds.Count > 0)
            {
                return await base.GetAll()
                    .Where(x => eventInstanceIds.Contains(x.EventInstanceId))
                    .CountAsync(cancellationToken: cancellationToken);
            }

            var orderId = _httpContextAccessor
                .HttpContext
                .User
                .FindFirst("order-id");

            if (orderId != null && !string.IsNullOrWhiteSpace(orderId.Value))
            {
                var canParse = Guid.TryParse(
                    orderId.Value,
                    out var orderIdParsed);

                if (canParse)
                {
                    return await base.GetAll()
                        .Where(x => x.OrderId == orderIdParsed)
                        .CountAsync(cancellationToken: cancellationToken);
                }
            }

            return default;
        }

        protected override IQueryable<Payment> GetAll()
        {
            if (_httpContextAccessor
                .HttpContext == null)
            {
                return default;
            }
            
            var isAdmin = _httpContextAccessor.IsAdmin();
            
            if (isAdmin)
            {
               return base.GetAll();
            }
            
            var eventInstanceIds = _eventInstanceClient.GetEventInstanceIdsForOrganiserId(_httpContextAccessor).GetAwaiter().GetResult();

            if (eventInstanceIds != null && eventInstanceIds.Count > 0)
            {
                return base
                    .GetAll()
                    .Where(x => eventInstanceIds.Contains(x.EventInstanceId));
            }

            var orderId = _httpContextAccessor
                .HttpContext
                .User
                .FindFirst("order-id");

            if (orderId!= null && !string.IsNullOrWhiteSpace(orderId.Value))
            {
                var canParse = Guid.TryParse(
                    orderId.Value,
                    out var orderIdParsed);

                if (canParse)
                {
                    return base
                        .GetAll()
                        .Where(x =>x.OrderId == orderIdParsed);
                }
            }

            return default;
        }
    }
}