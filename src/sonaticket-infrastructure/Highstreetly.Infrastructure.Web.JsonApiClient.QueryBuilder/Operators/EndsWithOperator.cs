namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class EndsWithOperator : IComparisonOperator
    {
        private readonly string _property;
        private readonly string _value;

        public EndsWithOperator(string property, string value)
        {
            _property = property;
            _value = value;
        }

        public override string ToString()
        {
            return $"endsWith({_property},'{_value}')";
        }
    }
}