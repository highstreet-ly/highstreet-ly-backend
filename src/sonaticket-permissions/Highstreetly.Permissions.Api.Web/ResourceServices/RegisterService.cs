using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Services;
using MassTransit;

namespace Highstreetly.Permissions.Api.Web.ResourceServices
{
    public class RegisterService : IResourceService<Register, string>
    {
        private readonly IBusClient _busControl;

        public RegisterService(
            IBusClient busControl)
        {
            _busControl = busControl;
        }

        public async Task<Register> CreateAsync(
            Register resource,
            CancellationToken cancellationToken)
        {
            var newId = NewId.NextGuid();

            resource.Id = newId.ToString();

            await _busControl.Send<IRegisterB2BUser>(new RegisterB2BUser
            {
                SourceId = newId,
                Email = resource.Email,
                UserName = resource.Email,
                Onboarding = resource.Onboarding,
                Password = resource.Password,
                Redirect = resource.Redirect,
                ConfirmPassword = resource.ConfirmPassword,
                createEventName = resource.createEventName,
                LastName = resource.LastName,
                FirstName = resource.FirstName,
                createEventStartDate = resource.createEventStartDate,
                CreateEventBusinessTypeId = resource.CreateEventBusinessTypeId
            });

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

        public Task<Register> UpdateAsync(
            string id,
            Register resource,
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

        public Task<IReadOnlyCollection<Register>> GetAsync(
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Register> GetAsync(
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
