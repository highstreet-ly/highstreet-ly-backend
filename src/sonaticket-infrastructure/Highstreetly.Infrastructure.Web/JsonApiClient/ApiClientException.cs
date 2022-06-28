using System;

namespace Highstreetly.Infrastructure.JsonApiClient
{
    public class ApiClientException : Exception
    {
        public string Url { get; }
        public string Method { get; }
        public string Reason { get; }

        public ApiClientException(string url, string method, string reason = null)
        {
            Url = url;
            Method = method;
            Reason = reason;
        }
    }
}