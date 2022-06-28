using System;
using System.Linq;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Queries.Expressions;
using JsonApiDotNetCore.Resources;

namespace Highstreetly.Management.Api.Web.ResourceDefinitions
{
    public class EventInstanceDefinition : JsonApiResourceDefinition<EventInstance, Guid>
    {
        public EventInstanceDefinition(IResourceGraph resourceGraph)
            : base(resourceGraph)
        {
        }

        public override FilterExpression OnApplyFilter(FilterExpression existingFilter)
        {
            var resourceContext = ResourceGraph.GetResourceContext<EventInstance>();

            var isSoftDeletedAttribute =
                resourceContext.Attributes.Single(instance =>
                    instance.Property.Name == nameof(EventInstance.Deleted));

            var isNotSoftDeleted = new ComparisonExpression(ComparisonOperator.Equals,
                new ResourceFieldChainExpression(isSoftDeletedAttribute),
                new LiteralConstantExpression(bool.FalseString));

            return existingFilter == null
                ? (FilterExpression)isNotSoftDeleted
                : new LogicalExpression(LogicalOperator.And,
                    new[] { isNotSoftDeleted, existingFilter });
        }
    }
}
