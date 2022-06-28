using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Resources;
using MassTransit;
using Microsoft.AspNetCore.Http;

namespace Highstreetly.Management.Api.Web.ResourceDefinitions
{
    public class ProductExtraGroupDefinition : JsonApiResourceDefinition<ProductExtraGroup, Guid>
    {
        private readonly ManagementDbContext _managementDbContext;
        private readonly IBusClient _busClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductExtraGroupDefinition(IResourceGraph resourceGraph, ManagementDbContext managementDbContext, IBusClient busClient, IHttpContextAccessor httpContextAccessor) : base(resourceGraph)
        {
            _managementDbContext = managementDbContext;
            _busClient = busClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task OnWriteSucceededAsync(ProductExtraGroup resource, OperationKind operationKind,
            CancellationToken cancellationToken)
        {
            switch (operationKind)
            {
               
                case OperationKind.DeleteResource:
                    // TicketTypeConfiguration ticketTypeConfiguration;
                    //
                    // var egdel = _managementDbContext
                    //     .ProductExtraGroup
                    //     .FirstOrDefault(x => x.Id == resource.Id);
                    //
                    // // if we are creating a new extra group then the relationship won't be set up yet
                    // // if we are deleting then we don't have any relational data at all - just the extra group ID
                    // // hence this:
                    // if (egdel == null)
                    // {
                    //     ticketTypeConfiguration = _managementDbContext
                    //         .TicketTypeConfigurations
                    //         .First(x => x.Id == resource.TicketTypeConfiguration.Id);
                    // }
                    // else
                    // {
                    //     ticketTypeConfiguration = _managementDbContext
                    //         .TicketTypeConfigurations
                    //         .First(x => x.Id == egdel.TicketTypeConfigurationId);
                    // }
                    //
                    // await _busClient.Publish<ITicketTypeUpdated>(new TicketTypeUpdated
                    // {
                    //     SourceId = resource.TicketTypeConfiguration?.Id ?? ticketTypeConfiguration.Id,
                    //     Name = ticketTypeConfiguration.Name,
                    //     CorrelationId = CorrelationId,
                    //     CommandSource = nameof(ProductExtraGroupDefinition)
                    // });
                    break;
                case OperationKind.UpdateResource:
                case OperationKind.CreateResource:

                    var eg = _managementDbContext
                        .ProductExtraGroup
                        .FirstOrDefault(x => x.Id == resource.Id);

                    await _busClient.Publish<ITicketTypeUpdated>(new TicketTypeUpdated
                    {
                        SourceId = resource.TicketTypeConfiguration?.Id ?? eg.TicketTypeConfigurationId.Value,
                        CorrelationId = CorrelationId,
                        CommandSource = nameof(ProductExtraGroupDefinition)
                    });
                    break;
                case OperationKind.SetRelationship:
                    break;
                case OperationKind.AddToRelationship:
                    break;
                case OperationKind.RemoveFromRelationship:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operationKind), operationKind, null);
            }
        }

        protected Guid CorrelationId
        {
            get
            {
                _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("x-correlation-id", out var correlationId);

                var canParse = Guid.TryParse(correlationId, out var parsed);

                if (correlationId.Count == 0 || string.IsNullOrWhiteSpace(correlationId) || !canParse)
                {
                    return NewId.NextGuid();
                }

                return parsed;
            }
        }
    }
}