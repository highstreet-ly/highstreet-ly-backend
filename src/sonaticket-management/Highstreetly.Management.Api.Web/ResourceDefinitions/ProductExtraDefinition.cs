using System;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Resources;
using MassTransit;
using Microsoft.AspNetCore.Http;

namespace Highstreetly.Management.Api.Web.ResourceDefinitions
{
    public class ProductExtraDefinition : JsonApiResourceDefinition<ProductExtra, Guid>
    {
        private readonly ManagementDbContext _managementDbContext;
        private readonly IBusClient _busClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductExtraDefinition(IResourceGraph resourceGraph, ManagementDbContext managementDbContext, IBusClient busClient, IHttpContextAccessor httpContextAccessor) : base(resourceGraph)
        {
            _managementDbContext = managementDbContext;
            _busClient = busClient;
            _httpContextAccessor = httpContextAccessor;
        }

       

        protected Guid CorrelationId
        {
            get
            {
                _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("x-correlation-id", out var correlationId);

                var canParse = Guid.TryParse(correlationId, out var parsed);

                if (correlationId.Count == 0 || string.IsNullOrWhiteSpace(correlationId) || !canParse)
                {
                    return NewId.NextGuid();
                }

                return parsed;
            }
        }
    }
}