using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentValidation;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Reservations.Distance;
using Highstreetly.Reservations.Resources;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Validators
{
    public class DraftOrderValidator : AbstractValidator<DraftOrder>
    {
        public DraftOrderValidator(
            IDistanceApi distanceApi,
            IJsonApiClient<EventInstance, Guid> eventInstanceViewModelClient,
            IJsonApiClient<TicketType, Guid> ticketTypesClient,
            IMapper mapper,
            ILogger<DraftOrderValidator> logger)
        {
    
            RuleSet("FinaliseOrder", () =>
            {
                RuleFor(x => x.OwnerEmail).NotEmpty().EmailAddress();
                RuleFor(x => x.DeliveryPostcode).CustomAsync(async (s, context, arg3) =>
                {
                    var entity = ((DraftOrder) context.InstanceToValidate);
                    var eventInstance = await eventInstanceViewModelClient.GetAsync(entity.EventInstanceId);
    
                    if (!eventInstance.IsNationalDelivery && entity.IsNationalDelivery)
                    {
                        context.AddFailure(
                            "IsNationalDelivery selected but this shop doesn't support that service type");
                    }
    
                    if (eventInstance.IsNationalDelivery && entity.IsNationalDelivery)
                    {
                        var distance = await distanceApi.IsWithinRange(eventInstance.PostCode, entity.DeliveryPostcode);
                        var distanceValueInMiles =
                            distance.Rows.FirstOrDefault()?.Elements.FirstOrDefault()?.Distance.Value / 1609;
    
                        logger.LogInformation($"Event instance delivery radius {eventInstance.DeliveryRadiusMiles}");
                        logger.LogInformation($"Shop postcode {eventInstance.PostCode}");
                        logger.LogInformation($"Delivery postcode {entity.DeliveryPostcode}");
                        logger.LogInformation($"Distance value in miles {distanceValueInMiles}");
    
                        if (distanceValueInMiles > eventInstance.DeliveryRadiusMiles)
                        {
                            var deliveryAddressIsOutsideOfTheDeliveryRadius =
                                "Delivery address is outside of the delivery radius";
                            logger.LogInformation(deliveryAddressIsOutsideOfTheDeliveryRadius);
                            context.AddFailure(deliveryAddressIsOutsideOfTheDeliveryRadius);
                        }
                    }
                });
            });
    
            RuleSet("Standard", () =>
            {
                RuleFor(x => x.DraftOrderItems).NotEmpty();
    
                RuleFor(x => x.DraftOrderItems).CustomAsync(async (lines, context, arg3) =>
                {
                    var entity = ((DraftOrder) context.InstanceToValidate);
    
                    //product extras need ids so that they can be matched and validated
                    var queryBuilder = new QueryBuilder().Equalz(
                        "event-instance-id",
                        entity.EventInstanceId.ToString());
    
    
                    var publishedTicketsQuery = await ticketTypesClient.GetListAsync(queryBuilder);
    
                    logger.LogInformation("Retrieved published ticket types");
    
                    var publishedTickets = publishedTicketsQuery.Select(s => new 
                    {
                        TicketType = mapper.Map<TicketType>(s),
                        OrderItem = new DraftOrderItem
                        {
                            TicketType = s.Id
                        },
                        AvailableQuantityForOrder = Math.Max(s.AvailableQuantity.GetValueOrDefault(), 0),
                        MaxSelectionQuantity = Math.Max(Math.Min(s.AvailableQuantity.GetValueOrDefault(), 20), 0),
                        PartiallyFulfilled = true
                    }).ToList();
                    
    
                    logger.LogInformation(
                        "checking that there are still enough available seats, and the seat type IDs submitted are valid");
                    // checks that there are still enough available seats, and the seat type IDs submitted are valid.
                    bool needsExtraValidation = false;
                    foreach (var seat in lines)
                    {
                        var modelItem = publishedTickets.FirstOrDefault(x => x.TicketType.Id == seat.TicketType);
    
                        if (modelItem != null)
                        {
                            if (seat.RequestedTickets > modelItem.AvailableQuantityForOrder)
                            {
                                //modelItem.PartiallyFulfilled = needsExtraValidation = true;
                                modelItem.OrderItem.ReservedTickets = modelItem.MaxSelectionQuantity;
                            }
                        }
                        else
                        {
                            logger.LogInformation(" ticket type no longer exists for event.");
                            needsExtraValidation = true;
                        }
                    }
    
                    if (needsExtraValidation)
                    {
                        context.AddFailure("Product is no longer available");
                    }
    
                    // now check that the number of extras items requested is valid
    
                    // foreach (var draftOrderItem in entity.Lines)
                    // {
                    //     foreach (var ticketProductExtraGroup in draftOrderItem.Ticket.ProductExtras)
                    //     {
                    //         var groupFromDb = (from pt in publishedTickets
                    //             from peg in pt.TicketType.ProductExtraGroups
                    //             where peg.GroupId == ticketProductExtraGroup.Id
                    //             select peg).Single();
                    //
                    //         var selectedInGroup = ticketProductExtraGroup.ProductExtras.Data.Where(x => x.Selected)
                    //             .Sum(x => x.ItemCount);
                    //
                    //         logger.LogInformation($"Validating group with {selectedInGroup} items selected");
                    //
                    //         if (selectedInGroup > groupFromDb.MaxSelectable)
                    //         {
                    //             context.AddFailure(
                    //                 $"Too many extras selected for group {groupFromDb.Name} you can select a maximum {groupFromDb.MaxSelectable} items.");
                    //         }
                    //
                    //         if (selectedInGroup < groupFromDb.MinSelectable)
                    //         {
                    //             context.AddFailure(
                    //                 $"Not enough extras selected for group {groupFromDb.Name} you can select a minimum {groupFromDb.MinSelectable} items.");
                    //         }
                    //     }
                    // }
                });
            });
        }
    }
}
