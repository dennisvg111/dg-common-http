using DG.Common.Caching;
using System;
using System.Net.Http;
using System.Runtime.Caching;

namespace DG.Common.Http.FluentBuilders
{
    /// <summary>
    /// Provides a custom constructor for the <see cref="HttpClient"/> class.
    /// </summary>
    public class HttpClientProvider
    {
        //We cache clients to prevent socket exhaustion, and cache it only for 5 minutes to prevent DNS problems.
        private const string _cacheName = "DG.Common.Http Client Provider Cache";
        private static readonly TypedCache<HttpClient> _cache = new TypedCache<HttpClient>(
            ExpirationPolicy.ForAbsoluteExpiration(TimeSpan.FromMinutes(5)),
            new MemoryCache(_cacheName)
        );
        private readonly Uulsid _cacheId = Uulsid.NewUulsid();

        private HttpMessageHandler _handler;



        /// <summary>
        /// Gets a <see cref="HttpClient"/> instance constructed by this <see cref="HttpClientProvider"/> Note that this may return the same instance if no new instance is needed.
        /// </summary>
        public HttpClient Client => _cache.GetOrCreate(_cacheId.ToString(), CreateNewClient);

        private HttpClient CreateNewClient()
        {
            var client = _handler == null ? new HttpClient() : new HttpClient(_handler);

            return client;
        }
    }
}
