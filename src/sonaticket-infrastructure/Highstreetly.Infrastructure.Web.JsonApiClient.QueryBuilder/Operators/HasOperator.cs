namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class HasOperator : IComparisonOperator
    {
        private readonly string _property;

        public HasOperator(string property)
        {
            _property = property;
        }

        public override string ToString()
        {
            return $"has({_property})";
        }
    }
}