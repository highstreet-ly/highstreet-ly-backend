using System.Linq;

namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class AnyOperator : IComparisonOperator
    {
        private readonly string _property;
        private readonly string[] _values;

        public AnyOperator(string property, params string[] values)
        {
            _property = property;
            _values = values;
        }

        public override string ToString()
        {
            return $"any({_property},{string.Join(',', _values.Select(x=> $"'{x}'").ToArray())})";
        }
    }
}