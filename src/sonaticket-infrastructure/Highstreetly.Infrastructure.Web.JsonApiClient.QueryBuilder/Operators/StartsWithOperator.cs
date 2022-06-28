namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class StartsWithOperator : IComparisonOperator
    {
        private readonly string _property;
        private readonly string _value;

        public StartsWithOperator(string property, string value)
        {
            _property = property;
            _value = value;
        }

        public override string ToString()
        {
            return $"startsWith({_property},'{_value}')";
        }
    }
}