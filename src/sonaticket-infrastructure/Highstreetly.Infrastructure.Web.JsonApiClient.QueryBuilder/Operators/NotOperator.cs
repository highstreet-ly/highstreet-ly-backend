namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class NotOperator : IComparisonOperator
    {
        private readonly IComparisonOperator _comparison;

        public NotOperator(IComparisonOperator comparison)
        {
            _comparison = comparison;
        }

        public override string ToString()
        {
            return $"not({_comparison})";
        }
    }
}