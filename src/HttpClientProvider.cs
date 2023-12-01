using DG.Common.Caching;
using System;
using System.Net.Http;
using System.Runtime.Caching;

namespace DG.Common.Http
{
    /// <summary>
    /// This class provides a custom way to create instances of <see cref="HttpClient"/> to prevent socket exhaustion and DNS caching problems.
    /// </summary>
    public static class HttpClientProvider
    {
        //We cache clients to prevent socket exhaustion, and cache it only for 5 minutes to prevent DNS problems.
        private const string _cacheName = "DG.Common.Http " + nameof(HttpClientProvider) + " Cache";
        private static readonly TypedCache<HttpClient> _cache = new TypedCache<HttpClient>(
            ExpirationPolicy.ForAbsoluteExpiration(TimeSpan.FromMinutes(5)),
            new MemoryCache(_cacheName)
        );

        /// <summary>
        /// <para>Gets a <see cref="HttpClient"/> instance constructed by the given <see cref="HttpClientSettings"/>.</para>
        /// <para>Note that this may return the same instance multiple times if no new instance is needed.</para>
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static HttpClient ClientForSettings(HttpClientSettings settings)
        {
            return _cache.GetOrCreate(settings.GetCacheId(), () => CreateNewClientForSettings(settings));
        }

        /// <summary>
        /// Returns a new instance of <see cref="HttpClient"/> for the given <see cref="HttpClientSettings"/>.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static HttpClient CreateNewClientForSettings(HttpClientSettings settings)
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
