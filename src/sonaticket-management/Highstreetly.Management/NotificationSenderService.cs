using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Configuration;
using Highstreetly.Infrastructure.Email;
using Highstreetly.Management.Resources;
using Highstreetly.Reservations.Contracts.Requests;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Highstreetly.Management
{
    public class NotificationSenderService : INotificationSenderService
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<NotificationSenderService> _logger;
        private readonly EmailTemplateOptions _emailOptions;

        public NotificationSenderService(
            IEmailSender emailSender,
            ILogger<NotificationSenderService> logger,
            TwilioConfiguration twilioConfiguration,
            EmailTemplateOptions emailOptions)
        {
            _emailSender = emailSender;
            _logger = logger;
            _emailOptions = emailOptions;
            TwilioClient.Init(
                twilioConfiguration.Sid,
                twilioConfiguration.AuthToken);
        }

        public async Task SendOrderConfirmedOperatorAsync(
            EventInstance evt,
            PricedOrder pricedOrder,
            Order order)
        {
            if (evt == null)
            {
                throw new ArgumentException("evt cannot be null");
            }

            if (pricedOrder == null)
            {
                throw new ArgumentException("order cannot be null");
            }

            try
            {
                _logger.LogInformation($"Lines count for SendOrderConfirmedAsync: {pricedOrder.PricedOrderLines.Count}");

                var ordersUrl =
                    $"{Environment.GetEnvironmentVariable("DASH_UI")}/u/product-list/{evt.Slug}/orders/{pricedOrder.OrderId}/edit";

                var vmData = new
                {
                    Url = ordersUrl,
                    pricedOrder.HumanReadableId,
                    evt.Name,
                    evt.Location,
                    pricedOrder.IsFreeOfCharge,
                    Tickets = pricedOrder.PricedOrderLines.Select(
                            x => new
                            {
                                x.Description,
                                x.Quantity,
                                UnitPrice = (Convert.ToDecimal(x.UnitPrice.GetValueOrDefault()) / 100)
                                    .ToString(
                                        "c",
                                        CultureInfo.CreateSpecificCulture("en-GB")),
                                LineTotal = (Convert.ToDecimal(x.LineTotal.GetValueOrDefault()) / 100)
                                    .ToString(
                                        "c",
                                        CultureInfo.CreateSpecificCulture("en-GB"))
                            })
                        .ToArray(),
                    Total = (Convert.ToDecimal(pricedOrder.Total.GetValueOrDefault()) / 100)
                        .ToString(
                            "c",
                            CultureInfo.CreateSpecificCulture("en-GB"))
                };

                if (!string.IsNullOrWhiteSpace(evt.NotificationEmail))
                {
                    await _emailSender.SendEmailAsync(
                        evt.NotificationEmail,
                        "A new order has been placed",
                        vmData,
                        _emailOptions.OrderInTheBagOperator);
                }

                if (!string.IsNullOrWhiteSpace(evt.NotificationPhone) && ValidatePhoneNumber(evt.NotificationPhone))
                {
                    var message = await MessageResource.CreateAsync(
                        body: $"New order placed {ordersUrl}",
                        from: new PhoneNumber("+447576447893"),
                        to: new PhoneNumber(evt.NotificationPhone)
                    );
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "SendOrderConfirmedOperatorAsync Threw");
            }
        }

        public async Task SendOrderConfirmedAsync(
            string email,
            EventInstance evt,
            PricedOrder pricedOrder,
            Order order)
        {
            if (evt == null)
            {
                throw new ArgumentException("evt cannot be null");
            }

            if (pricedOrder == null)
            {
                throw new ArgumentException("priced order cannot be null");
            }

            if (order == null)
            {
                throw new ArgumentException("order cannot be null");
            }

            try
            {
                _logger.LogInformation($"Lines count for SendOrderConfirmedAsync: {pricedOrder.PricedOrderLines.Count}");

                var vmData = new
                {
                    pricedOrder.HumanReadableId,
                    evt.Name,
                    evt.Location,
                    pricedOrder.IsFreeOfCharge,
                    ToTable = pricedOrder.IsToTable.HasValue && pricedOrder.IsToTable.Value ? pricedOrder.TableInfo : "",
                    Tickets = pricedOrder.PricedOrderLines.Select(
                            x => new
                            {
                                x.Description,
                                x.Quantity,
                                UnitPrice = (Convert.ToDecimal(x.UnitPrice.GetValueOrDefault()) / 100)
                                    .ToString(
                                        "c",
                                        CultureInfo.CreateSpecificCulture("en-GB")),
                                LineTotal = (Convert.ToDecimal(x.LineTotal.GetValueOrDefault()) / 100)
                                    .ToString(
                                        "c",
                                        CultureInfo.CreateSpecificCulture("en-GB"))
                            })
                        .ToArray(),
                    Total = (Convert.ToDecimal(pricedOrder.Total.GetValueOrDefault()) / 100)
                        .ToString(
                            "c",
                            CultureInfo.CreateSpecificCulture("en-GB"))
                };

                var message = $"We've received your Order {order.HumanReadableId}";

                if (!string.IsNullOrWhiteSpace(order.OwnerPhone) && ValidatePhoneNumber(order.OwnerPhone))
                {
                    var sms = await MessageResource.CreateAsync(
                        body: message,
                        from: new PhoneNumber("+447576447893"),
                        to: new PhoneNumber(order.OwnerPhone)
                    );
                }

                await _emailSender.SendEmailAsync(
                    email,
                    message,
                    vmData,
                    _emailOptions.OrderInTheBag);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "SendOrderConfirmedAsync Threw");
            }
        }

        public async Task SendOrderRefundedEmail(
            string email,
            EventInstance evt,
            PricedOrder pricedOrder,
            string hrOrderId,
            string url,
            long refundedAmount)
        {
            try
            {
                await _emailSender.SendEmailAsync(
                    email,
                    "Your order has been refunded",
                    new
                    {
                        Title = $"Your order #{hrOrderId} has been refunded",
                        ButtonText = "View receipt",
                        Text = "Use the link below to view your refund receipt",
                        ButtonUrl = url,
                        evt.Location,
                        RefundedAmount = (Convert.ToDecimal(refundedAmount) / 100)
                            .ToString(
                                "c",
                                CultureInfo.CreateSpecificCulture("en-GB")),
                        evt.Name,
                        pricedOrder.IsFreeOfCharge,
                        Tickets = pricedOrder.PricedOrderLines.Select(
                                x => new
                                {
                                    x.Description,
                                    x.Quantity,
                                    UnitPrice = (Convert.ToDecimal(x.UnitPrice.GetValueOrDefault()) / 100)
                                        .ToString(
                                            "c",
                                            CultureInfo.CreateSpecificCulture("en-GB")),
                                    LineTotal = (Convert.ToDecimal(x.LineTotal.GetValueOrDefault()) / 100)
                                        .ToString(
                                            "c",
                                            CultureInfo.CreateSpecificCulture("en-GB"))
                                })
                            .ToArray(),
                        Total = (Convert.ToDecimal(pricedOrder.Total.GetValueOrDefault()) / 100)
                            .ToString(
                                "c",
                                CultureInfo.CreateSpecificCulture("en-GB"))
                    },
                    _emailOptions.OrderRefunded);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "SendOrderRefundedEmail Threw");
            }
        }

        public async Task SendOrderProcessingCompleteAsync(
            string email,
            EventInstance evt,
            PricedOrder pricedOrder,
            Order order)
        {
            if (order == null)
            {
                throw new ArgumentException("order cannot be null");
            }

            try
            {
                var note = string.Empty;

                if (order.OrderAdvisoryDate.HasValue)
                {
                    note =
                        $"Your order {order.HumanReadableId} is ready to collect after {order.OrderAdvisoryDate.Value:dddd, dd MMMM yyyy} {order.OrderAdvisoryTimeOfDay}. Message from operator: {order.CustomerDispatchAdvisory}";
                }
                else
                {
                    note =
                        $"Your order {order.HumanReadableId} is ready to collect. Message from operator: {order.CustomerDispatchAdvisory}";
                }

                var vmData = new
                {
                    CustomerDispatchAdvisory =
                        note,
                    pricedOrder.IsFreeOfCharge,
                    evt.Name,
                    evt.Location,
                    Tickets = pricedOrder.PricedOrderLines.Select(
                            x => new
                            {
                                x.Description,
                                x.Quantity,
                                UnitPrice = (Convert.ToDecimal(x.UnitPrice.GetValueOrDefault()) / 100)
                                    .ToString(
                                        "c",
                                        CultureInfo.CreateSpecificCulture("en-GB")),
                                LineTotal = (Convert.ToDecimal(x.LineTotal.GetValueOrDefault()) / 100)
                                    .ToString(
                                        "c",
                                        CultureInfo.CreateSpecificCulture("en-GB"))
                            })
                        .ToArray(),
                    Total = (Convert.ToDecimal(pricedOrder.Total.GetValueOrDefault()) / 100)
                        .ToString(
                            "c",
                            CultureInfo.CreateSpecificCulture("en-GB"))
                };

                var message = $"Your Order {order.HumanReadableId} is Ready!";


                if (pricedOrder.IsToTable.HasValue && pricedOrder.IsToTable.Value)
                {
                    message = $"Your Order {order.HumanReadableId} for table # {pricedOrder.TableInfo} is Ready!";
                }

                var smsMessage = $"{message} - Please check your email for info. ";

                if (!string.IsNullOrWhiteSpace(order.CustomerDispatchAdvisory))
                {
                    smsMessage += $"Dispatch note: {order.CustomerDispatchAdvisory}";
                }

                if (!string.IsNullOrWhiteSpace(order.OwnerPhone) && ValidatePhoneNumber(order.OwnerPhone))
                {
                    var sms = await MessageResource.CreateAsync(
                        body: smsMessage,
                        from: new PhoneNumber("+447576447893"),
                        to: new PhoneNumber(order.OwnerPhone)
                    );
                }

                await _emailSender.SendEmailAsync(
                    email,
                    message,
                    vmData,
                    _emailOptions.OrderProcessingComplete);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "SendOrderProcessingCompleteAsync Threw");
            }
        }

        public async Task SendOrderProcessingAsync(
            string email,
            EventInstance evt,
            PricedOrder pricedOrder,
            Order order)
        {
            if (order == null)
            {
                throw new ArgumentException("order cannot be null");
            }

            if (email == null)
            {
                throw new ArgumentException("email cannot be null");
            }

            try
            {
                var vmData = new
                {
                    Text = $"Your order {order.HumanReadableId} is being processed",
                    pricedOrder.IsFreeOfCharge,
                    evt.Location,
                    evt.Name,
                    Tickets = pricedOrder.PricedOrderLines.Select(
                             x => new
                             {
                                 x.Description,
                                 x.Quantity,
                                 UnitPrice = (Convert.ToDecimal(x.UnitPrice.GetValueOrDefault()) / 100)
                                     .ToString(
                                         "c",
                                         CultureInfo.CreateSpecificCulture("en-GB")),
                                 LineTotal = (Convert.ToDecimal(x.LineTotal.GetValueOrDefault()) / 100)
                                     .ToString(
                                         "c",
                                         CultureInfo.CreateSpecificCulture("en-GB"))
                             })
                         .ToArray(),
                    Total = (Convert.ToDecimal(pricedOrder.Total.GetValueOrDefault()) / 100)
                         .ToString(
                             "c",
                             CultureInfo.CreateSpecificCulture("en-GB"))
                };
                if (!string.IsNullOrWhiteSpace(order.OwnerPhone) && ValidatePhoneNumber(order.OwnerPhone))
                {
                    var phoneNumber = new PhoneNumber(order.OwnerPhone);

                    var sms = await MessageResource.CreateAsync(
                        body: $"Your order {order.HumanReadableId} is being processed",
                        from: new PhoneNumber("+447576447893"),
                        to: phoneNumber
                    );
                }

                await _emailSender.SendEmailAsync(
                    email,
                    $"Your order {order.HumanReadableId} is being processed",
                    vmData,
                    _emailOptions.OrderProcessing);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool ValidatePhoneNumber(
            string number)
        {
            return Regex.Match(
                    number,
                    @"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$",
                    RegexOptions.IgnoreCase)
                .Success;
        }
    }
}