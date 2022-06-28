using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Web.ResourceServices
{
    public class ConfirmEmailService : IResourceService<ConfirmEmail, string>
    {
        private readonly ILogger<ConfirmEmailService> _logger;
        private readonly UserManager<User> _userManager;

        public ConfirmEmailService(
            ILogger<ConfirmEmailService> logger,
            UserManager<User> userManager) 
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<ConfirmEmail> CreateAsync(
            ConfirmEmail resource,
            CancellationToken cancellationToken)
        {

            _logger.LogInformation($"Executing {nameof(CreateAsync)}");
            resource.Id = NewId.NextGuid().ToString();

            var user = await _userManager.FindByIdAsync(resource.UserId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{resource.UserId}'.");
            }

            await _userManager.ConfirmEmailAsync(user, resource.Code);

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

        public Task<ConfirmEmail> UpdateAsync(
            string id,
            ConfirmEmail resource,
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

        public Task<IReadOnlyCollection<ConfirmEmail>> GetAsync(
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ConfirmEmail> GetAsync(
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