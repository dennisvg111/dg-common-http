using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// This class exists only to provide a single implementation for the <see cref="HttpRequestMessageBuilder.Get"/> and <see cref="HttpRequestMessageBuilder.Post"/> methods, and should not be called directly.
    /// </summary>
    public class HttpRequestMethod
    {
        private static readonly ConcurrentDictionary<HttpMethod, HttpRequestMethod> _methods;

        private readonly HttpMethod _method;

        private HttpRequestMethod(HttpMethod method)
        {
            _method = method;
        }

        internal static HttpRequestMethod For(HttpMethod method)
        {
            return _methods.GetOrAdd(method, (m) => new HttpRequestMethod(m));
        }

        public HttpRequestMessageBuilder To(string url)
        {
            return new HttpRequestMessageBuilder(_method, new UriBuilder(url));
        }

        public HttpRequestMessageBuilder To(Uri url)
        {
            return new HttpRequestMessageBuilder(_method, new UriBuilder(url));
        }

        public HttpRequestMessageBuilder To(UriBuilder urlBuilder)
        {
            return new HttpRequestMessageBuilder(_method, urlBuilder);
        }
    }
}
