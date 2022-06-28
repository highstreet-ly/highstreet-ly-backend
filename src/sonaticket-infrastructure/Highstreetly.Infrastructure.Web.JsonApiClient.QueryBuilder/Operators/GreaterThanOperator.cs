namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class GreaterThanOperator : IComparisonOperator
    {
        private readonly string _property;
        private readonly string _value;

        public GreaterThanOperator(string property, string value)
        {
            _property = property;
            _value = value;
        }

        public override string ToString()
        {
            return $"greaterThan({_property},'{_value}')";
        }
    }
}