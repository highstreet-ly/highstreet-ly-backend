namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class EqualsOperator : IComparisonOperator
    {
        private readonly string _property;
        private readonly string _value;

        public EqualsOperator(string property, string value)
        {
            _property = property;
            _value = value;
        }

        public override string ToString()
        {
            // special case for null
            if (_value != "null")
            {
                return $"equals({_property},'{_value}')";
            }
            else
            {
                return $"equals({_property},{_value})";
            }
           
        }
    }
}
