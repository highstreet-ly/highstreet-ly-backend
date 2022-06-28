namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder
{
    public static class StringExtensions
    {
        public static bool HasQuery(
            this string uriToBeAppended)
        {
            var queryIndex = uriToBeAppended.IndexOf('?');
            return queryIndex != -1;
        }
    }
}