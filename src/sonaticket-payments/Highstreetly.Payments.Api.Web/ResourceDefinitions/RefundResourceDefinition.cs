using System;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Payments.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Resources;

namespace Highstreetly.Payments.Api.Web.ResourceDefinitions
{
    public class RefundResourceDefinition : JsonApiResourceDefinition<Refund, Guid>
    {
        private readonly IBusClient _busClient;
        
        public RefundResourceDefinition(IResourceGraph resourceGraph, IBusClient busClient) : base(resourceGraph)
        {
            _busClient = busClient;
        }


        public override Task OnPrepareWriteAsync(Refund resource, OperationKind operationKind, CancellationToken cancellationToken)
        {
            if (operationKind == OperationKind.CreateResource)
            {
                resource.DateCreated = DateTime.UtcNow;
            }
            
            return base.OnPrepareWriteAsync(resource, operationKind, cancellationToken);
        }

        public override async Task OnWriteSucceededAsync(Refund resource, OperationKind operationKind, CancellationToken cancellationToken)
        {
            switch (operationKind)
            {
                case OperationKind.CreateResource:

                    await _busClient.Send<IIssueRefund>(
                        new IssueRefund
                        {
                            ChargeId = resource.ChargeId,
                            RefundId = resource.Id
                        });
                    
                    break;
                case OperationKind.UpdateResource:
                    break;
                case OperationKind.DeleteResource:
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
            
            await base.OnWriteSucceededAsync(resource, operationKind, cancellationToken);
        }
    }
}