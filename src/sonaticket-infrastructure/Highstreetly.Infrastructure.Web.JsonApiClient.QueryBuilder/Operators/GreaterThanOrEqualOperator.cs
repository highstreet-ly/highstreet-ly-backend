namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class GreaterThanOrEqualOperator : IComparisonOperator
    {
        private readonly string _property;
        private readonly string _value;

        public GreaterThanOrEqualOperator(string property, string value)
        {
            _property = property;
            _value = value;
        }

        public override string ToString()
        {
            return $"greaterOrEqual({_property},'{_value}')";
        }
    }
}