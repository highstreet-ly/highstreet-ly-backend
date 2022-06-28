using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Configuration;
using Highstreetly.Infrastructure.Email;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Highstreetly.Permissions
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

        public Task SendForgotPasswordEmail(string email, string url)
        {
            return _emailSender.SendEmailAsync(email, "Your Magic Link", new
            {
                Title = "Reset your password",
                ButtonText = "Reset your password!",
                Text = "Use the link below to reset your password",
                ButtonUrl = url,
            }, _emailOptions.ForgotPassword);
        }

        public Task SendEmailAlreadyExistsAsync(string email)
        {
            var redirectPort = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? ":4200"
                : "";

            var domain = Environment.GetEnvironmentVariable("A_RECORD");
            var domainTld = Environment.GetEnvironmentVariable("DOMAIN_TLD");
            var url = $"https://dashboard.{domain}.{domainTld}{redirectPort}/login";

            return _emailSender.SendEmailAsync(email, "You're already registered", new
            {
                Title = "Someone tried to create a new registration using this email address",
                ButtonText = "Your Dashboard",
                Text = "Someone tried to create a new registration using this email address. If you'd like to create a new event you can do this using your dashboard.",
                Url = url,
                RedirectUrl = url,
            }, _emailOptions.MagicLink);
        }

        public Task SendPasswordWasResetEmail(string userEmail)
        {
            var port = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? ":4201"
                : "";

            var domain = Environment.GetEnvironmentVariable("A_RECORD");
            var domainTld = Environment.GetEnvironmentVariable("DOMAIN_TLD");

            return _emailSender.SendEmailAsync(userEmail, "Your Magic Link", new
            {
                Title = "Your password has been reset",
                ButtonText = "Login",
                Text = $"Your password has been reset. If you didn't do this please contact us@{domain}.{domainTld}",
                ButtonUrl = $"https://dashboard.{domain}.{domainTld}{port}"
            }, _emailOptions.PasswordReset);
        }

        public Task SendWelcomeEapEmail(string email, string url)
        {
            return _emailSender.SendEmailAsync(email, "Welcome to the highstreet.ly early access program", new
            {
                Title = "Welcome to the highstreet.ly early access program!",
                ButtonText = "Set your password",
                Text = "Welcome to the highstreet.ly early access program, Use the link below to set your password <br> <b>We need to offer link to the blog, FAQ and intercom support methods",
                ButtonUrl = url,
            }, _emailOptions.MagicLink);
        }

        public Task SendEmailConfirmationAsync(

            string email,
            string userId,
            string code,
            string redirect)
        {
            var callbackUrl = $"{redirect}?code={WebUtility.UrlEncode(code)}&userId={userId}";

            return _emailSender.SendEmailAsync(email, "Confirm your email", new
            {
                Url = callbackUrl,
                UserId = userId,
                Code = HttpUtility.UrlEncode(code)
            }, _emailOptions.Registration);
        }

    }
}