using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Payments.Resources;
using Highstreetly.Reservations.Contracts.Requests;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Queries.Expressions;
using JsonApiDotNetCore.Resources;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Polly;
using Polly.Retry;

namespace Highstreetly.Payments.Api.Web.ResourceDefinitions
{
    public class PaymentResourceDefinition : JsonApiResourceDefinition<Payment, Guid>
    {
        private readonly IBusClient _busClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJsonApiClient<PricedOrder, Guid> _pricedOrderClient;
        private readonly AsyncRetryPolicy _waitForOrderToBePriced;
        private readonly IJwtService _jwtService;

        public PaymentResourceDefinition(
            IResourceGraph resourceGraph,
            IBusClient busClient, 
            IHttpContextAccessor httpContextAccessor,
            IJsonApiClient<PricedOrder, Guid> pricedOrderClient,
            IJwtService jwtService) : base(resourceGraph)
        {
            _busClient = busClient;
            _httpContextAccessor = httpContextAccessor;
            _pricedOrderClient = pricedOrderClient;
            _jwtService = jwtService;

            _waitForOrderToBePriced = Policy
                                      .Handle<OrderNotPricedException>()
                                      .WaitAndRetryAsync(new[]
                                                         {
                                                             TimeSpan.FromSeconds(1),
                                                             TimeSpan.FromSeconds(2),
                                                             TimeSpan.FromSeconds(3),
                                                             TimeSpan.FromSeconds(10),
                                                             TimeSpan.FromSeconds(30),
                                                         });
        }

        // unless the user is admin or owns the payment they won't see it anyway so allow all fields
        // public override SparseFieldSetExpression OnApplySparseFieldSet

        public override async Task OnWriteSucceededAsync(Payment resource, OperationKind operationKind, CancellationToken cancellationToken)
        {
            switch (operationKind)
            {
                case OperationKind.CreateResource:
                    break;
                case OperationKind.UpdateResource:
                    var commands = _httpContextAccessor.HttpContext?.Request.Headers["Command-Type"].ToList() ?? new List<string>();
                    foreach (var command in commands)
                    {
                        switch (command)
                        {
                            case "InitiateThirdPartyProcessorPayment":
                                var pricedOrder = await _waitForOrderToBePriced.ExecuteAsync(() =>
                                    GetPricedOrderAsync(resource.OrderId.ToString()));

                                var command2 = CreatePaymentCommand(pricedOrder, resource.EventInstanceId, resource);
                                await _busClient.Send<IInitiateThirdPartyProcessorPayment>(command2);
                                break;
                        }
                    }

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
        
        private async Task<PricedOrder> GetPricedOrderAsync(string orderId)
        {
            var queryBuilder = new QueryBuilder()
                .Equalz(
                    "order-id",
                    orderId)
                .Includes("priced-order-lines");

            var priced = await _pricedOrderClient.GetListAsync(queryBuilder);

            var pricedOrderAsync = priced.ToList();
            if (pricedOrderAsync.FirstOrDefault() != null )
            {
                if (pricedOrderAsync.First().Total == 0)
                {
                    throw new OrderNotPricedException();
                }
            }

            return priced.First();
        }
        
        private InitiateThirdPartyProcessorPayment CreatePaymentCommand(PricedOrder order, Guid eventInstanceInstanceId, Payment payment)
        {
            var description = $"Payment for Order: {order.HumanReadableId}, Service: {eventInstanceInstanceId}";
            var totalAmount = order.Total;

            var paymentCommand =
                new InitiateThirdPartyProcessorPayment
                {
                    PaymentIntentClientSecret = payment.PaymentIntentSecret,
                    PaymentIntentId = payment.PaymentIntentId,
                    PaymentId = payment.Id,
                    EventInstanceId = eventInstanceInstanceId,
                    PaymentSourceId = order.OrderId,
                    Description = description,
                    TotalAmount = totalAmount.GetValueOrDefault(),
                    CorrelationId = CorrelationId,
                    Items = new List<InitiateThirdPartyProcessorPayment.PaymentItem>
                            {
                                new()
                                {
                                    Amount = totalAmount.GetValueOrDefault() - order.DeliveryFee.GetValueOrDefault() -
                                             order.PaymentPlatformFees.GetValueOrDefault() -
                                             order.PlatformFees.GetValueOrDefault(),
                                    Description = "Sub total"
                                },
                                new()
                                {
                                    Amount = order.DeliveryFee.GetValueOrDefault(),
                                    Description = "DeliveryFee"
                                },
                                new()
                                {
                                    Amount = order.PaymentPlatformFees.GetValueOrDefault(),
                                    Description = "PaymentPlatformFees"
                                },
                                new()
                                {
                                    Amount = order.PlatformFees.GetValueOrDefault(),
                                    Description = "PlatformFees"
                                }
                            }
                };

            return paymentCommand;
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

                        var canParse = Guid.TryParse(claimOrderId.Value, out var oid);
                        if (canParse)
                        {
                            orderId = oid;
                        }
                        return canParse;
                    });
            }

            return orderId;
        }

        protected Guid CorrelationId
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    throw new ArgumentException("_httpContextAccessor.HttpContext");
                }

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