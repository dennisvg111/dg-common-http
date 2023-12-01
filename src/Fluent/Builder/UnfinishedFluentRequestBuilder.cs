using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace DG.Common.Http.Fluent.Builder
{
    /// <summary>
    /// This class exists only to provide a single implementation for the <see cref="FluentRequest.Get"/> and <see cref="FluentRequest.Post"/> methods, and should not be called directly.
    /// </summary>
    internal class UnfinishedFluentRequestBuilder : IFluentRequestBuilder
    {
        private static readonly ConcurrentDictionary<string, UnfinishedFluentRequestBuilder> _methods = new ConcurrentDictionary<string, UnfinishedFluentRequestBuilder>();

        private readonly HttpMethod _method;

        private UnfinishedFluentRequestBuilder(HttpMethod method)
        {
            _method = method;
        }

        internal static UnfinishedFluentRequestBuilder For(HttpMethod method)
        {
            return _methods.GetOrAdd(method.Method, (m) => new UnfinishedFluentRequestBuilder(method));
        }

        public FluentRequest To(string url)
        {
            return new FluentRequest(_method, new Uri(url, UriKind.RelativeOrAbsolute));
        }

        public FluentRequest To(Uri url)
        {
            return new FluentRequest(_method, url);
        }

        public FluentRequest To(UriBuilder urlBuilder)
        {
            return new FluentRequest(_method, urlBuilder.Uri);
        }
    }
}
