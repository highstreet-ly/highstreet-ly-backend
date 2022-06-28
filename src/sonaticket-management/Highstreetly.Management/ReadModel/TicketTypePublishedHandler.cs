using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Management.Resources;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class TicketTypePublishedHandler : IConsumer<ITicketTypePublished>
    {
        private readonly ManagementDbContext _managementDbContext;
        private readonly IBusClient _ticketReservationClient;
        private readonly ILogger<TicketTypePublishedHandler> _logger;

        public TicketTypePublishedHandler(
            ManagementDbContext managementDbContext,
            IBusClient ticketReservationClient,
            ILogger<TicketTypePublishedHandler> logger)
        {
            _managementDbContext = managementDbContext;
            _ticketReservationClient = ticketReservationClient;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<ITicketTypePublished> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                _logger.LogInformation($"Running ConsumeContext<ITicketTypePublished>");
                var ticketTypePublished = @event.Message;
                _logger.LogInformation($"Publishing ticket type {@event.Message.SourceId}");
                try
                {
                    var ticketType =
                        _managementDbContext.TicketTypes.FirstOrDefault(x => x.Id == ticketTypePublished.SourceId);

                    var ticketTypeConfiguration =
                        await _managementDbContext
                            .TicketTypeConfigurations
                            .Include(x => x.ProductCategory)
                            .Include(x => x.ProductExtraGroups)
                            .ThenInclude(x => x.ProductExtras)
                            .FirstAsync(x => x.Id == ticketTypePublished.SourceId);

                    if (ticketType != null)
                    {
                        ticketType.IsPublished = true;
                        ticketType.Name = ticketTypeConfiguration.Name;
                        ticketType.Description = ticketTypeConfiguration.Description;
                        ticketType.Price = ticketTypeConfiguration.Price;
                        ticketType.Quantity = ticketTypeConfiguration.Quantity;
                        ticketType.Images = ticketTypeConfiguration.Images;
                        ticketType.SortOrder = ticketTypeConfiguration.SortOrder;

                        ticketType.ProductCategoryId = ticketTypeConfiguration.ProductCategoryId;

                        // this is probably wrong
                        // should be getting the items and updating iff needed

                        var productExtraGroups = (from peg in ticketTypeConfiguration.ProductExtraGroups
                            select new ProductExtraGroup
                            {
                                Description = peg.Description,
                                Name = peg.Name,
                                Id = NewId.NextGuid(),
                                TicketTypeId = ticketType.Id,
                                MaxSelectable = peg.MaxSelectable,
                                MinSelectable = peg.MinSelectable,
                                SortOrder = peg.SortOrder,
                            }).ToList();

                        _managementDbContext.ProductExtraGroup.AddRange(productExtraGroups);

                        var productExtras = (from peg in ticketTypeConfiguration.ProductExtraGroups
                            from pe in peg.ProductExtras
                            select new ProductExtra
                            {
                                Description = pe.Description,
                                ProductExtraGroupId = productExtraGroups.First(x => x.Name == peg.Name)
                                    .Id,
                                Name = pe.Name,
                                Price = pe.Price,
                                Selected = pe.Selected,
                                Id = NewId.NextGuid(),
                                ItemCount = pe.ItemCount,
                                SortOrder = pe.SortOrder
                            }).ToList();

                        _managementDbContext.ProductExtras.AddRange(productExtras);

                        await _managementDbContext.SaveChangesAsync();

                        if (ticketType.Quantity < ticketTypeConfiguration.Quantity)
                        {
                            await _ticketReservationClient.Send<IAddTicketTypes>(
                                new AddTicketTypes
                                {
                                    EventInstanceId = ticketType.EventInstanceId,
                                    TicketType = ticketType.Id,
                                    Quantity = ticketType.Quantity ?? 0,
                                    CorrelationId = @event.Message.CorrelationId
                                });
                        }

                        if (ticketType.Quantity > ticketTypeConfiguration.Quantity)
                        {
                            await _ticketReservationClient.Send<IRemoveTicketTypes>(
                                new AddTicketTypes
                                {
                                    EventInstanceId = ticketType.EventInstanceId,
                                    TicketType = ticketType.Id,
                                    Quantity = ticketType.Quantity ?? 0,
                                    CorrelationId = @event.Message.CorrelationId
                                });
                        }
                    }
                    else
                    {

                        ticketType = new TicketType
                        {
                            Id = ticketTypeConfiguration.Id,
                            EventInstanceId = ticketTypeConfiguration.EventInstanceId,
                            Name = ticketTypeConfiguration.Name,
                            Description = ticketTypeConfiguration.Description,
                            Price = ticketTypeConfiguration.Price,
                            Quantity = ticketTypeConfiguration.Quantity,
                            IsPublished = true,
                            MainImageId = ticketTypeConfiguration.MainImageId,
                            Images = ticketTypeConfiguration.Images,
                            Tags = ticketTypeConfiguration.Tags,
                            SortOrder = ticketTypeConfiguration.SortOrder,
                            ProductCategoryId = ticketTypeConfiguration.ProductCategoryId,
                            ProductExtraGroups = ticketTypeConfiguration
                                .ProductExtraGroups
                                .Select(x =>
                                    new ProductExtraGroup
                                    {
                                        Description = x.Description,
                                        Name = x.Name,
                                        Id = NewId.NextGuid(),
                                        MaxSelectable = x.MaxSelectable,
                                        MinSelectable = x.MinSelectable,
                                        SortOrder = x.SortOrder,
                                        ProductExtras = x.ProductExtras.Select(pe => new ProductExtra
                                            {
                                                Description = pe.Description,
                                                Name = pe.Name,
                                                Price = pe.Price,
                                                Selected = pe.Selected,
                                                Id = NewId.NextGuid(),
                                                ItemCount = pe.ItemCount,
                                                SortOrder = pe.SortOrder,
                                            })
                                            .ToList()
                                    })
                                .ToList()
                        };

                        await _managementDbContext.TicketTypes.AddAsync(ticketType);

                        await _managementDbContext.SaveChangesAsync();

                        await _ticketReservationClient.Send<IAddTicketTypes>(
                            new AddTicketTypes
                            {
                                EventInstanceId = ticketType.EventInstanceId,
                                TicketType = ticketType.Id,
                                Quantity = ticketType.Quantity ?? 0,
                                CorrelationId = @event.Message.CorrelationId
                            });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Couldn't run ITicketTypePublished for {ticketTypePublished.Name}", ex);
                    throw;
                }
            }
        }
    }
}