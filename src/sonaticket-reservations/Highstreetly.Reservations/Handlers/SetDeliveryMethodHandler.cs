using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Highstreetly.Reservations.Handlers
{
    public class SetDeliveryMethodHandler : IConsumer<ISetDeliveryMethod>
    {
        private readonly ILogger<SetDeliveryMethodHandler> _logger;
        private readonly IBusClient _busClient;
        private readonly IEventSourcedRepository<Order> _repository;
        private readonly IPricingService _pricingService;
        private readonly ReservationDbContext _reservationDbContext;
        private RetryPolicy _waitForOrder;

        public SetDeliveryMethodHandler(
            IEventSourcedRepository<Order> repository,
            IPricingService pricingService,
            IBusClient busClient,
            ILogger<SetDeliveryMethodHandler> logger,
            ReservationDbContext reservationDbContext)
        {
            _repository = repository;
            _pricingService = pricingService;
            _busClient = busClient;
            _logger = logger;
            _reservationDbContext = reservationDbContext;
            _waitForOrder = Policy
                            .Handle<InvalidOperationException>()
                            .WaitAndRetry(new[]
                                          {
                                              TimeSpan.FromSeconds(1),
                                              TimeSpan.FromSeconds(2),
                                              TimeSpan.FromSeconds(3),
                                          });
            
        }

        public async Task Consume(
            ConsumeContext<ISetDeliveryMethod> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.SourceId }))
            {
                _logger.LogInformation(
                    $"public Task Consume(ConsumeContext<AddTicketsToBasket> command) for order {command.Message.SourceId} and event {command.Message.EventInstanceId}");

                try
                {
                    //var items = command.Message.Tickets.Select(t => new OrderItem(t.TicketType, t.Quantity, t.TicketDetails)).ToList();
                    var pricedOrder = _waitForOrder.Execute(()=> _reservationDbContext
                                                                 .PricedOrders
                                                                 .Include(x => x.PricedOrderLines)
                                                                 .ThenInclude(x => x.ProductExtras)
                                                                 .Single(x => x.OrderId == command.Message.SourceId));

                    pricedOrder.IsLocalDelivery = command.Message.IsLocalDelivery;
                    pricedOrder.IsNationalDelivery = command.Message.IsNationalDelivery;
                    pricedOrder.IsClickAndCollect = command.Message.IsClickAndCollect;
                    pricedOrder.IsToTable = command.Message.IsToTable;
                    pricedOrder.TableInfo = command.Message.TableInfo;
                    // pricedOrder.MakeSubscription = draftOrder.MakeSubscription;

                    var items = pricedOrder.PricedOrderLines.Select(t => new OrderItem(t.TicketType, t.Quantity,
                            new OrderTicketDetails
                            {
                                Id = t.Id,
                                Name = t.Name,
                                Price = t.UnitPrice,
                                Quantity = t.Quantity,
                                DisplayName = t.Description,
                                ProductExtras = t
                                    .ProductExtras
                                    .Select(pe => new ProductExtra
                                    {
                                        Description = pe.Description,
                                        Id = pe.Id,
                                        Name = pe.Name,
                                        Price = pe.Price,
                                        Selected = pe.Selected,
                                        ItemCount = pe.ItemCount,
                                        ReferenceProductExtraId = pe.ReferenceProductExtraId,
                                    })
                                    .ToList()
                            }))
                        .ToList();

                    var order = _repository.Find(command.Message.SourceId);

                    var totals = await _pricingService.CalculateTotal(
                        command.Message.EventInstanceId,
                        command.Message.SourceId,
                        Order.ConvertItems(items));

                    order.SetDeliveryMethod(items, totals);

                    await _repository.Save(order, command.Message.CorrelationId.ToString());
                    await _reservationDbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Couldn't run ISetDeliveryMethod  {command.Message.SourceId}", ex);
                    _logger.LogError(ex.ToString());
                    throw;
                }
            }
        }
    }
}