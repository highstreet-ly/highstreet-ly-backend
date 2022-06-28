using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Highstreetly.Reservations.Handlers
{
    public class AddTicketsToBasketHandler : IConsumer<IAddTicketsToBasket>
    {
        private readonly ILogger<AddTicketsToBasketHandler> _logger;
        private readonly IEventSourcedRepository<Order> _repository;
        private readonly IPricingService _pricingService;
        private readonly ReservationDbContext _reservationDbContext;
        private RetryPolicy _waitForOrder;
        private RetryPolicy _waitForEsOrder;

        public AddTicketsToBasketHandler(
            ILogger<AddTicketsToBasketHandler> logger,
            IEventSourcedRepository<Order> repository,
            IPricingService pricingService,
            ReservationDbContext reservationDbContext)
        {
            _logger = logger;
            _repository = repository;
            _pricingService = pricingService;
            _reservationDbContext = reservationDbContext;
            
            _waitForOrder = Policy
                            .Handle<InvalidOperationException>()
                            .WaitAndRetry(new[]
                                               {
                                                   TimeSpan.FromSeconds(1),
                                                   TimeSpan.FromSeconds(2),
                                                   TimeSpan.FromSeconds(3),
                                               });
            
           _waitForEsOrder = Policy
                .Handle<NullReferenceException>()
                .WaitAndRetry(new[]
                              {
                                  TimeSpan.FromSeconds(1),
                                  TimeSpan.FromSeconds(2),
                                  TimeSpan.FromSeconds(3),
                              });
        }

        public async Task Consume(
            ConsumeContext<IAddTicketsToBasket> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.Id }))
            {
                _logger.LogInformation(
                    $"public Task Consume(ConsumeContext<AddTicketsToBasket> command) for order {command.Message.OrderId} and event {command.Message.EventInstanceId}");
                try
                {
                    //var items = command.Message.Tickets.Select(t => new OrderItem(t.TicketType, t.Quantity, t.TicketDetails)).ToList();
                    var draftOrder = _waitForOrder.Execute(()=> _reservationDbContext
                                                                .DraftOrders
                                                                .Include(x => x.DraftOrderItems)
                                                                .ThenInclude(x => x.Ticket)
                                                                .ThenInclude(x => x.ProductExtras)
                                                                .Single(x => x.OrderId == command.Message.OrderId));

                    var items = draftOrder.DraftOrderItems.Select(t => new OrderItem(t.TicketType, t.RequestedTickets,
                            new OrderTicketDetails
                            {
                                Id = t.Ticket.Id,
                                Name = t.Ticket.Name,
                                Price = t.Ticket.Price,
                                Quantity = t.Ticket.Quantity,
                                DisplayName = t.Ticket.DisplayName,
                                ProductExtras = t
                                    .Ticket
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

                    var order = _waitForEsOrder.Execute(() =>  _repository.Find(command.Message.OrderId));

                    var totals = await _pricingService.CalculateTotal(
                        command.Message.EventInstanceId,
                        command.Message.OrderId,
                        Order.ConvertItems(items));
                    
                    order.UpdateSeats(items, totals);

                    await _repository.Save(order, command.Message.CorrelationId.ToString());
                }
                catch (System.Exception ex)
                {
                    _logger.LogError($"Couldn't run IAddTicketsToBasket  {command.Message.OrderId}", ex);
                    _logger.LogError(ex, ex.ToString());
                    throw;
                }
            }
        }
    }
}