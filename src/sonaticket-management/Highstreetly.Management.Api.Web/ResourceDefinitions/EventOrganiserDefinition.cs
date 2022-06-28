using System;
using System.Security.Claims;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Queries.Expressions;
using JsonApiDotNetCore.Resources;
using Microsoft.AspNetCore.Http;

namespace Highstreetly.Management.Api.Web.ResourceDefinitions
{
    public class EventOrganiserDefinition : JsonApiResourceDefinition<EventOrganiser, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventOrganiserDefinition(IResourceGraph resourceGraph, IHttpContextAccessor httpContextAccessor)
            : base(resourceGraph)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override SparseFieldSetExpression OnApplySparseFieldSet(
            SparseFieldSetExpression existingSparseFieldSet)
        {
            if (_httpContextAccessor.HttpContext?.User.FindFirstValue("access-all") != "true")
            {
                return existingSparseFieldSet
                    .Excluding<EventOrganiser>(
                        eventOrganiser => eventOrganiser.StripeAccessToken, ResourceGraph)
                    .Excluding<EventOrganiser>(
                        eventOrganiser => eventOrganiser.StripeAccountId, ResourceGraph)
                    .Excluding<EventOrganiser>(
                        eventOrganiser => eventOrganiser.StripeCode, ResourceGraph)
                    .Excluding<EventOrganiser>(
                        eventOrganiser => eventOrganiser.StripePublishableKey, ResourceGraph);
            }

            return existingSparseFieldSet;
        }
    }
}