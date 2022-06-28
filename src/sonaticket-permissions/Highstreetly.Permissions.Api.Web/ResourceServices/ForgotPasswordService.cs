using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Email;
using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Errors;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Serialization.Objects;
using JsonApiDotNetCore.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Web.ResourceServices
{
    public class ForgotPasswordService : IResourceService<ForgotPassword, string>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ForgotPasswordService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly EmailTemplateOptions _emailTemplateOptions;
        private INotificationSenderService _notificationSender;

        public ForgotPasswordService(
            UserManager<User> userManager,
            ILogger<ForgotPasswordService> logger,
            IEmailSender emailSender,
            EmailTemplateOptions emailTemplateOptions, INotificationSenderService notificationSender) 
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _emailTemplateOptions = emailTemplateOptions;
            _notificationSender = notificationSender;
        }

        public async Task<ForgotPassword> CreateAsync(
            ForgotPassword resource,
            CancellationToken cancellationToken)
        {

            _logger.LogInformation($"Executing {nameof(CreateAsync)}");

            resource.Id = NewId.NextGuid().ToString();


            var user = await _userManager.FindByEmailAsync(resource.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                throw new JsonApiException(new Error(HttpStatusCode.UnprocessableEntity));
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{resource.Redirect}?code={WebUtility.UrlEncode(code)}&userId={user.Id}";

            await _notificationSender.SendForgotPasswordEmail(user.Email, callbackUrl);

            return resource;
        }

        public Task AddToToManyRelationshipAsync(
            string primaryId,
            string relationshipName,
            ISet<IIdentifiable> secondaryResourceIds,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ForgotPassword> UpdateAsync(
            string id,
            ForgotPassword resource,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRelationshipAsync(
            string primaryId,
            string relationshipName,
            object secondaryResourceIds,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(
            string id,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromToManyRelationshipAsync(
            string primaryId,
            string relationshipName,
            ISet<IIdentifiable> secondaryResourceIds,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<ForgotPassword>> GetAsync(
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ForgotPassword> GetAsync(
            string id,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetRelationshipAsync(
            string id,
            string relationshipName,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetSecondaryAsync(
            string id,
            string relationshipName,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}