using System;
using System.Net.Http;
using JustEat.HttpClientInterception;
using Microsoft.Extensions.Http;

namespace Highstreetly.Management.Tests
{
    /// <summary>
    /// A class that registers an intercepting HTTP message handler at the end of
    /// the message handler pipeline when an <see cref="HttpClient"/> is created.
    /// </summary>
    public sealed class HttpClientInterceptionFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly HttpClientInterceptorOptions _options;

        public HttpClientInterceptionFilter(HttpClientInterceptorOptions options)
        {
            _options = options;
        }

        /// <inheritdoc/>
        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return (builder) =>
            {
                // Run any actions the application has configured for itself
                next(builder);

                // Add the interceptor as the last message handler
                builder.AdditionalHandlers.Add(_options.CreateHttpMessageHandler());
            };
        }
    }
}