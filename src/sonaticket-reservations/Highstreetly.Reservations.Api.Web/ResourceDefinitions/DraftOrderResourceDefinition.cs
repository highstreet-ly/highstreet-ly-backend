using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Permissions.Contracts.Requests;
using Highstreetly.Reservations.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Errors;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Serialization.Objects;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Retry;
using Claim = System.Security.Claims.Claim;
using OrderTicketDetails = Highstreetly.Infrastructure.MessageDtos.OrderTicketDetails;
using ProductExtra = Highstreetly.Infrastructure.MessageDtos.ProductExtra;

namespace Highstreetly.Reservations.Api.Web.ResourceDefinitions
{
    public class DraftOrderResourceDefinition : JsonApiResourceDefinition<DraftOrder, Guid>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IBusClient _busClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;
        private readonly ILogger<DraftOrderResourceDefinition> _logger;
        private readonly ReservationDbContext _reservationDbContext;
        private readonly IJsonApiClient<User, Guid> _userApiClient;
        private readonly IValidator<DraftOrder> _validator;
        private readonly RetryPolicy _waitForOrder;
        private readonly IMemoryCache _cache;

        public DraftOrderResourceDefinition(
            IResourceGraph resourceGraph,
            IJwtService jwtService,
            IConfiguration configuration,
            IBusClient busClient,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IValidator<DraftOrder> validator,
            ILogger<DraftOrderResourceDefinition> logger,
            IJsonApiClient<User, Guid> userApiClient,
            ReservationDbContext reservationDbContext,
            IMemoryCache cache) : base(resourceGraph)
        {
            _jwtService = jwtService;
            _configuration = configuration;
            _busClient = busClient;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _validator = validator;
            _logger = logger;
            _userApiClient = userApiClient;
            _reservationDbContext = reservationDbContext;
            _cache = cache;
            _waitForOrder = Policy
                .Handle<InvalidOperationException>()
                .WaitAndRetry(
                    new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(3)
                    });
        }

        private Guid CorrelationId
        {
            get
            {
                _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(
                    "x-correlation-id",
                    out var correlationId);

                var canParse = Guid.TryParse(
                    correlationId,
                    out var parsed);

                if (correlationId.Count == 0 || string.IsNullOrWhiteSpace(correlationId) || !canParse)
                {
                    return NewId.NextGuid();
                }

                return parsed;
            }
        }

        public override async Task OnWritingAsync(
            DraftOrder resource,
            OperationKind operationKind,
            CancellationToken cancellationToken)
        {
            switch (operationKind)
            {
                case OperationKind.CreateResource:
                    resource.HumanReadableId = RandomIdGenerator.GetBase36(5); // 1 in 60466176 chance of collision
                    break;
                case OperationKind.UpdateResource:

                    // validate using FinaliseOrder
                    if (!string.IsNullOrEmpty(resource.DeliveryPostcode))
                    {
                        var validationResult = await _validator
                            .ValidateAsync(
                                resource,
                                strategy => strategy.IncludeRuleSets("FinaliseOrder"),
                                cancellationToken);
                        
                        if (!validationResult.IsValid)
                        {
                            throw new JsonApiException(
                                new Error(HttpStatusCode.Conflict)
                                {
                                    Title = "Validation Errors.",
                                    Detail = string.Join(
                                        @", ",
                                        validationResult.Errors.Select(x => x.ErrorMessage)
                                            .ToArray())
                                });
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
                    throw new ArgumentOutOfRangeException(
                        nameof(operationKind),
                        operationKind,
                        null);
            }

            await ProcessCommands(resource);

            await base.OnWritingAsync(
                resource,
                operationKind,
                cancellationToken);
        }

        public override async Task OnWriteSucceededAsync(
            DraftOrder resource,
            OperationKind operationKind,
            CancellationToken cancellationToken)
        {
            switch (operationKind)
            {
                case OperationKind.CreateResource:
                    // we don't care that the token isn't persisted since it is just for validating the user during the order process

                    var identityUrl = _configuration.GetIdsUrl();
                    var audience = _configuration.GetSection("IdentityServer")["Audience"];

                    var claims = CreateClaimsIdentitiesAsync(resource);

                    var cached = _cache.TryGetValue<X509Certificate2>(
                        "jwtCertificate",
                        out var certificate);

                    if (!cached)
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions();
                         cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromSeconds(6000));
                         cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(6000);
                         certificate = _jwtService.LoadCertificate();
                         _cache.Set(
                             "jwtCertificate",
                             certificate,
                             cacheEntryOptions);
                    }


                    resource.UserToken = _jwtService.CreateRequestJwt(
                        claims,
                        identityUrl,
                        audience,
                        new X509SigningCredentials(certificate));

                    await _busClient.Publish<IDraftOrderCreated>(
                        new DraftOrderCreated
                        {
                            OrderId = resource.Id,
                            EventInstanceId = resource.EventInstanceId,
                            CorrelationId = CorrelationId,
                            HumanReadableId = resource.HumanReadableId,
                            IsLocalDelivery = resource.IsLocalDelivery,
                            IsNationalDelivery = resource.IsNationalDelivery,
                            IsClickAndCollect = resource.IsClickAndCollect,
                            IsToTable = resource.IsToTable ?? false,
                            TableInfo = resource.TableInfo
                        });
                    break;
                case OperationKind.UpdateResource:
                    await _busClient.Publish<IDraftOrderUpdated>(
                        new DraftOrderUpdated
                        {
                            OrderId = resource.Id,
                            CorrelationId = CorrelationId
                        });
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
                    throw new ArgumentOutOfRangeException(
                        nameof(operationKind),
                        operationKind,
                        null);
            }

            await base.OnWriteSucceededAsync(
                resource,
                operationKind,
                cancellationToken);
        }

        public async Task ProcessCommands(
            DraftOrder resource)
        {
            var commands = _httpContextAccessor.HttpContext?.Request.Headers["Command-Type"]
                .ToList() ?? new List<string>();
            foreach (var command in commands)
                switch (command)
                {
                    case "AssignRegistrant":
                        if (!string.IsNullOrEmpty(
                            resource.OwnerEmail) /* && draftOrder.OwnerEmail != resource.OwnerEmail*/)
                        {
                            User owner;

                            _logger.LogInformation($"Looking for user with email {resource.OwnerEmail}");

                            var queryBuilder = new QueryBuilder()
                                .Equalz(
                                    "normalized-email",
                                    resource.OwnerEmail.ToUpper());

                            var users = await _userApiClient.GetListAsync(queryBuilder, true);

                            var applicationUserViewModels = users.ToList();

                            _logger.LogInformation(
                                $"Found {applicationUserViewModels.Count()} users with email {resource.OwnerEmail}");

                            if (!applicationUserViewModels.Any())
                            {
                                _logger.LogInformation($"Creating user {resource.OwnerEmail}");
                                var ownerNameSplit = resource.OwnerName.Split(" ");
                                owner = await _userApiClient.CreateAsync(
                                    new User
                                    {
                                        Email = resource.OwnerEmail,
                                        NormalizedEmail = resource.OwnerEmail.ToUpper(),
                                        EmailConfirmed = true,
                                        FirstName = ownerNameSplit[0],
                                        LastName = ownerNameSplit.Length > 1 ? ownerNameSplit[1] : ""
                                    }, true);
                            }
                            else
                            {
                                owner = applicationUserViewModels.First();
                            }

                            resource.OwnerId = owner.Id;

                            await _busClient.Publish<IAssignRegistrantDetails>(
                                new AssignRegistrantDetails
                                {
                                    OrderId = resource.OrderId,
                                    Email = owner.Email,
                                    UserId = owner.Id,
                                    CorrelationId = CorrelationId,
                                    DeliveryPostcode = resource.DeliveryPostcode,
                                    DeliveryLine1 = resource.DeliveryLine1,
                                    Phone = resource.OwnerPhone,
                                    OwnerName = resource.OwnerName
                                });
                        }

                        break;
                    case "SetDeliveryMethod":
                        var cmd = new SetDeliveryMethod
                        {
                            SourceId = resource.Id,
                            CorrelationId = CorrelationId,
                            EventInstanceId = resource.EventInstanceId
                        };

                        cmd.IsClickAndCollect = resource.IsClickAndCollect;
                        cmd.IsLocalDelivery = resource.IsLocalDelivery;
                        cmd.IsNationalDelivery = resource.IsNationalDelivery;
                        cmd.IsToTable = resource.IsToTable ?? false;
                        cmd.TableInfo = resource.TableInfo;


                        await _busClient.Publish<ISetDeliveryMethod>(cmd);

                        break;
                    case "CommitOrder":
                        await _busClient.Publish<ICommitOrder>(
                            new CommitOrder
                            {
                                SourceId = resource.Id,
                                CorrelationId = CorrelationId,
                                Tickets = resource.DraftOrderItems
                                    .Select(
                                        x => new TicketQuantity(
                                            x.TicketType,
                                            x.RequestedTickets,
                                            new OrderTicketDetails
                                            {
                                                Id = x.Ticket.Id,
                                                Name = x.Ticket.Name,
                                                Price = x.Ticket.Price,
                                                Quantity = x.Ticket.Quantity,
                                                DisplayName = x.Ticket.DisplayName,
                                                EventInstanceId = x.Ticket.EventInstanceId,
                                                ProductExtras = x.Ticket.ProductExtras.Select(
                                                        pe =>
                                                            new ProductExtra
                                                            {
                                                                Description = pe.Description,
                                                                Name = pe.Name,
                                                                Price = pe.Price,
                                                                Selected = pe.Selected,
                                                                Id = pe.Id,
                                                                ReferenceProductExtraId = pe.ReferenceProductExtraId,
                                                                ItemCount = pe.ItemCount
                                                            })
                                                    .ToList()
                                            }))
                                    .ToList()
                            });
                        break;

                    case "AddTicketsToBasket":
                        var draftOrder = _waitForOrder.Execute(
                            () => _reservationDbContext
                                .DraftOrders
                                .Include(x => x.DraftOrderItems)
                                .ThenInclude(x => x.Ticket)
                                .ThenInclude(x => x.ProductExtras)
                                .Single(x => x.OrderId == resource.Id));

                        await _busClient.Publish<IAddTicketsToBasket>(
                            new AddTicketsToBasket
                            {
                                OrderId = draftOrder.Id,
                                OrderVersion = draftOrder.OrderVersion,
                                OwnerId = draftOrder.OwnerId.GetValueOrDefault(),
                                EventInstanceId = draftOrder.EventInstanceId,
                                Tickets = draftOrder.DraftOrderItems
                                    .Select(
                                        x => new TicketQuantity(
                                            x.TicketType,
                                            x.RequestedTickets,
                                            new OrderTicketDetails
                                            {
                                                Id = x.Ticket.Id,
                                                Name = x.Ticket.Name,
                                                Price = x.Ticket.Price,
                                                Quantity = x.Ticket.Quantity,
                                                DisplayName = x.Ticket.DisplayName,
                                                EventInstanceId = x.Ticket.EventInstanceId,
                                                ProductExtras = x.Ticket.ProductExtras.Select(
                                                        pe => new ProductExtra
                                                        {
                                                            Description = pe.Description,
                                                            Name = pe.Name,
                                                            Price = pe.Price,
                                                            Selected = pe.Selected,
                                                            Id = pe.Id,
                                                            ReferenceProductExtraId = pe.ReferenceProductExtraId,
                                                            ItemCount = pe.ItemCount
                                                        })
                                                    .ToList()
                                            }))
                                    .ToList(),
                                CorrelationId = CorrelationId,
                                HumanReadableId = draftOrder.HumanReadableId
                            });
                        break;
                }
        }

        private ClaimsIdentity CreateClaimsIdentitiesAsync(
            DraftOrder entity)
        {
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(
                new Claim(
                    "order-id",
                    entity.OrderId.ToString()));
            return claimsIdentity;
        }
    }
}