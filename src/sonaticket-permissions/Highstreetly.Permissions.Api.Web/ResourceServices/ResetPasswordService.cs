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
    public class ResetPasswordService : IResourceService<ResetPassword, string>
    {
        private readonly ILogger<ResetPasswordService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly INotificationSenderService _notificationSender;

        public ResetPasswordService(
            ILogger<ResetPasswordService> logger,
            UserManager<User> userManager,
            INotificationSenderService notificationSender)
        {
            _logger = logger;
            _userManager = userManager;
            _notificationSender = notificationSender;
        }

        public async Task<ResetPassword> CreateAsync(
            ResetPassword resource,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Executing {nameof(CreateAsync)}");
            resource.Id = NewId.NextGuid().ToString();

            var user = await _userManager.FindByIdAsync(resource.UserId);
            if (user == null)
            {
                throw new JsonApiException(new Error(HttpStatusCode.UnprocessableEntity));
            }

            var result = await _userManager.ResetPasswordAsync(user, resource.Code, resource.Password);

            if (result.Succeeded)
            {
                await _notificationSender.SendPasswordWasResetEmail(user.Email);
                return resource;
            }

            throw new JsonApiException(new Error(HttpStatusCode.UnprocessableEntity));
        }

        public Task AddToToManyRelationshipAsync(
            string primaryId,
            string relationshipName,
            ISet<IIdentifiable> secondaryResourceIds,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResetPassword> UpdateAsync(
            string id,
            ResetPassword resource,
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

        public Task<IReadOnlyCollection<ResetPassword>> GetAsync(
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResetPassword> GetAsync(
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