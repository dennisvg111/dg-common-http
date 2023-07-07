using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// This class exists only to provide a single implementation for the <see cref="FluentRequest.Get"/> and <see cref="FluentRequest.Post"/> methods, and should not be called directly.
    /// </summary>
    public class FluentRequestMethod
    {
        private static readonly ConcurrentDictionary<string, FluentRequestMethod> _methods;

        private readonly HttpMethod _method;

        private FluentRequestMethod(HttpMethod method)
        {
            _method = method;
        }

        internal static FluentRequestMethod For(HttpMethod method)
        {
            return _methods.GetOrAdd(method.Method, (m) => new FluentRequestMethod(method));
        }

        /// <summary>
        /// Specifies the url to execute this method on.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public FluentRequest To(string url)
        {
            return new FluentRequest(_method, new UriBuilder(url));
        }

        /// <summary>
        /// Specifies the url to execute this method on.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public FluentRequest To(Uri url)
        {
            return new FluentRequest(_method, new UriBuilder(url));
        }

        /// <summary>
        /// Specifies the url (using <see cref="UriBuilder.Uri"/>) to execute this method on.
        /// </summary>
        /// <param name="urlBuilder"></param>
        /// <returns></returns>
        public FluentRequest To(UriBuilder urlBuilder)
        {
            return new FluentRequest(_method, urlBuilder);
        }
    }
}
