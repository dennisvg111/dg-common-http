using DG.Common.Caching;
using System;
using System.Net.Http;

namespace DG.Common.Http
{
    /// <summary>
    /// This class provides a custom way to create cached instances of <see cref="HttpClient"/> to prevent socket exhaustion and DNS caching problems.
    /// </summary>
    public class HttpClientProvider
    {
        private const string _sharedCacheName = "DG.Common.Http " + nameof(HttpClientProvider) + " Cache";

        //We cache clients to prevent socket exhaustion, and cache it only for 5 minutes to prevent DNS problems.
        private static readonly ExpirationPolicy _expirationPolicy = ExpirationPolicy.ForAbsoluteExpiration(TimeSpan.FromMinutes(5));

        private readonly ITypedCache<HttpClient> _cache;

        public HttpClientProvider(ITypedCache<HttpClient> cache)
        {
            _cache = cache;
        }

        public HttpClientProvider(ICacheFactory cacheFactory)
        {
            var provider = new CacheProvider(cacheFactory);
            _cache = provider.Named<HttpClient>(_sharedCacheName);
        }

        /// <summary>
        /// <para>Gets a <see cref="HttpClient"/> instance constructed by the given <see cref="HttpClientSettings"/>.</para>
        /// <para>Note that this may return the same instance multiple times if no new instance is needed.</para>
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public HttpClient ClientForSettings(HttpClientSettings settings)
        {
            return _cache.GetOrCreate(settings.GetCacheId(), () => CreateNewClientForSettings(settings), _expirationPolicy);
        }

        /// <summary>
        /// Returns a new instance of <see cref="HttpClient"/> for the given <see cref="HttpClientSettings"/>.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public HttpClient CreateNewClientForSettings(HttpClientSettings settings)
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                UseCookies = settings.UseCookies,
                AllowAutoRedirect = settings.MaxRedirects > 0
            };
            if (settings.MaxRedirects > 0)
            {
                handler.MaxAutomaticRedirections = settings.MaxRedirects;
            }

            return new HttpClient(handler, true)
            {
                BaseAddress = settings.BaseAddress,
                Timeout = settings.MaxTimeout
            };
        }
    }
}
