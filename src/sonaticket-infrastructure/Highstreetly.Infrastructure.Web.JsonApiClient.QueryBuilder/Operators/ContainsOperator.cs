namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class ContainsOperator : IComparisonOperator
    {
        private readonly string _property;
        private readonly string _value;

        public ContainsOperator(string property, string value)
        {
            _property = property;
            _value = value;
        }

        public override string ToString()
        {
            return $"contains({_property},'{_value}')";
        }
    }
}