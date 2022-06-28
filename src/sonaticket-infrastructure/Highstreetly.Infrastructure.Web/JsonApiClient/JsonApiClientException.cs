using System;

namespace Highstreetly.Infrastructure.JsonApiClient
{
    public class JsonApiClientException : Exception
    {
        public string URL { get; set; }

        public JsonApiClientException(string url, Exception exception) : base("Request Failed", exception)
        {
            this.URL = url;
        }
    }
}