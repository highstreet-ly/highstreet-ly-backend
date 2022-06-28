using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Errors;
using JsonApiDotNetCore.Hooks;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Serialization.Objects;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Services
{
    public class OrderService : JsonApiResourceService<Order, Guid>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IBusClient _busClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(
            IResourceRepositoryAccessor repositoryAccessor,
            IQueryLayerComposer queryLayerComposer,
            IPaginationContext paginationContext,
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IJsonApiRequest request,
            IResourceChangeTracker<Order> resourceChangeTracker,
            IResourceHookExecutorFacade hookExecutor,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IBusClient busClient) : base(repositoryAccessor, queryLayerComposer, paginationContext, options,
            loggerFactory, request, resourceChangeTracker, hookExecutor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _busClient = busClient;
        }

        public override async Task<Order> UpdateAsync(Guid id, Order resource, CancellationToken cancellationToken)
        {
            var command = _httpContextAccessor.HttpContext.Request.Headers["Command-Type"]
                                              .Single();

            switch (command)
            {
                case "SetOrderProcessing":
                    await _busClient.Send<ISetOrderProcessing>(new SetOrderProcessing
                                                               {
                                                                   OrderId = resource.Id
                                                               });
                    break;
                case "SetOrderProcessingComplete":
                    resource.CustomerDispatchAdvisory = resource.CustomerDispatchAdvisory;
                    await base.UpdateAsync(id, resource, cancellationToken);
                    await _busClient.Send<ISetOrderProcessingComplete>(new SetOrderProcessingComplete
                                                                       {
                                                                           OrderId = resource.Id
                                                                       });
                    break;

                // case "IssueRefund":
                //     await base.UpdateAsync(id, resource, cancellationToken);
                //     await _busClient.Send<IIssueRefund>(new IssueRefund
                //                                         {
                //                                             OrderId = resource.Id,
                //                                             PaymentId = resource.PaymentId.Value
                //                                         });
                //     break;
                //
                // case "IssueItemRefund":
                //     await base.UpdateAsync(id, resource, cancellationToken);
                //     await _busClient.Send<IIssueRefund>(new IssueRefund
                //                                         {
                //                                             OrderId = resource.Id,
                //                                             PaymentId = resource.PaymentId.Value
                //                                         });
                //     break;
                // case "IssuePartialRefund":
                //     
                //     await base.UpdateAsync(id, resource, cancellationToken);
                //     await _busClient.Send<IIssueRefund>(new IssueRefund
                //                                         {
                //                                             OrderId = resource.Id,
                //                                             PaymentId = resource.PaymentId.Value
                //                                         });
                //     break;
            }

            return resource;
        }
    }
}