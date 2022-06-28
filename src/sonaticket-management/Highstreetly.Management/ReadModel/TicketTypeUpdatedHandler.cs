using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Management.Resources;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class TicketTypeUpdatedHandler : IConsumer<ITicketTypeUpdated>
    {
        private readonly ManagementDbContext _managementDbContext;
        private readonly ILogger<TicketTypeUpdatedHandler> _logger;
        private readonly IBusClient _ticketReservationClient;

        public TicketTypeUpdatedHandler(
            ILogger<TicketTypeUpdatedHandler> logger,
            ManagementDbContext managementDbContext,
            IBusClient ticketReservationClient)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
            _ticketReservationClient = ticketReservationClient;
        }

        public async Task Consume(
            ConsumeContext<ITicketTypeUpdated> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                _logger.LogInformation(
                    $"Running ConsumeContext ITicketTypeUpdated  - command source: {@event.Message.CommandSource}");

                try
                {
                    var ticketType = _managementDbContext
                        .TicketTypes
                        .Include(x => x.ProductExtraGroups)
                        .ThenInclude(x => x.ProductExtras)
                        .Include(x => x.Images)
                        .FirstOrDefault(x => x.Id == @event.Message.SourceId);

                    var ticketTypeConfiguration =
                        await _managementDbContext
                            .TicketTypeConfigurations
                            .Include(x => x.ProductExtraGroups)
                            .ThenInclude(x => x.ProductExtras)
                            .Include(x => x.Images)
                            .FirstAsync(x => x.Id == @event.Message.SourceId);



                    if (ticketType != null)
                    {
                        ticketType.IsPublished = true;
                        ticketType.Name = ticketTypeConfiguration.Name;
                        ticketType.Description = ticketTypeConfiguration.Description;
                        ticketType.Price = ticketTypeConfiguration.Price;
                        ticketType.Quantity = ticketTypeConfiguration.Quantity;
                        ticketType.MainImageId = ticketTypeConfiguration.MainImageId;
                        ticketType.Group = ticketTypeConfiguration.Group;
                        ticketType.Tags = ticketTypeConfiguration.Tags;
                        ticketType.SortOrder = ticketTypeConfiguration.SortOrder;
                        ticketType.ProductCategoryId = ticketTypeConfiguration.ProductCategoryId;
                        ticketType.Images = ticketTypeConfiguration.Images;

                        var extras = ticketType.ProductExtraGroups.SelectMany(x => x.ProductExtras);
                        var groups = ticketType.ProductExtraGroups;

                        foreach (var productExtra in extras)
                        {
                            _managementDbContext.ProductExtras.Remove(productExtra);
                        }

                        foreach (var productExtraGroup in groups)
                        {
                            _managementDbContext.ProductExtraGroup.Remove(productExtraGroup);
                        }

                        await _managementDbContext.SaveChangesAsync();

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
                                SortOrder = pe.SortOrder,
                            }).ToList();

                        _managementDbContext.ProductExtras.AddRange(productExtras);

                        await _managementDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogInformation(
                            $"Failed to locate ticket type read model being updated with id {@event.Message.SourceId}. It was probably never published?");
                    }

                    // this needs to happen whether or not the product is published
                    // NOTE: this is on hold - user product subs needs more thought
                    if (@event.Message.UpdatePrice)
                    {
                        // var evt = _managementDbContext
                        //     .EventInstances
                        //     .First(x => x.Id == ticketTypeConfiguration.EventInstanceId);
                        //
                        // var org = _managementDbContext
                        //     .EventOrganisers
                        //     .First(x => x.Id == evt.EventOrganiserId);

                        // Stripe.StripeConfiguration.ApiKey = org.StripeAccessToken;
                        //
                        // var productService = new ProductService();
                        // var priceService = new PriceService();
                        //
                        // var productUpdateOptions = new Stripe.ProductUpdateOptions()
                        // {
                        //     Name = ticketTypeConfiguration.Name,
                        // };
                        //
                        // await productService
                        //     .UpdateAsync(
                        //         ticketTypeConfiguration.Metadata["stripe-product-id"],
                        //         productUpdateOptions);
                        //
                        // var priceUpdateDeactivateOptions = new Stripe.PriceUpdateOptions
                        // {
                        //     Active = false
                        // };
                        //
                        // await priceService
                        //     .UpdateAsync(
                        //         ticketTypeConfiguration.Metadata["stripe-price-id"],
                        //         priceUpdateDeactivateOptions);
                        //
                        // var priceCreateOptions = new PriceCreateOptions
                        // {
                        //     Product = ticketTypeConfiguration.Metadata["stripe-product-id"],
                        //     UnitAmount = (long?)ticketTypeConfiguration.Price,
                        //     Currency = "gbp"
                        // };
                        //
                        // var price = await priceService.CreateAsync(priceCreateOptions);
                        //
                        // var md = ticketTypeConfiguration.Metadata;
                        //
                        // md["stripe-price-id"] =  price.Id;
                        //
                        // ticketTypeConfiguration.Metadata = md;
                        //
                        // await _managementDbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Couldn't run ITicketTypeUpdated for {@event.Message.SourceId}", ex);
                    throw;
                }
            }
        }
    }
}