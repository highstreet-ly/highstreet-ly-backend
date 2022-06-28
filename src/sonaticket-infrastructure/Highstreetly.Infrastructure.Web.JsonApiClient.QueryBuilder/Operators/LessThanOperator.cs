namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class LessThanOperator : IComparisonOperator
    {
        private readonly string _property;
        private readonly string _value;

        public LessThanOperator(string property, string value)
        {
            _property = property;
            _value = value;
        }

        public override string ToString()
        {
            return $"lessThan({_property},'{_value}')";
        }
    }
}