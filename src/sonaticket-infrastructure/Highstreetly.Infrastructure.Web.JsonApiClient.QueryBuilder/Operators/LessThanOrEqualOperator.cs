namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class LessThanOrEqualOperator : IComparisonOperator
    {
        private readonly string _property;
        private readonly string _value;

        public LessThanOrEqualOperator(string property, string value)
        {
            _property = property;
            _value = value;
        }

        public override string ToString()
        {
            return $"lessOrEqual({_property},'{_value}')";
        }
    }
}