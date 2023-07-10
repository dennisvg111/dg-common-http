using System;
using System.Net.Http;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Provides a custom constructor for the <see cref="HttpClient"/> class.
    /// </summary>
    public class HttpClientSettings
    {
        /// <summary>
        /// The default maximum number of redirects that a request will follow automatically.
        /// </summary>
        public const int DefaultAutomaticRedirectLimit = 50;

        private readonly Uulsid _cacheId = Uulsid.NewUulsid();
        internal string CacheId => _cacheId.ToString();

        private readonly int _maxRedirects;
        private readonly bool _useCookies;
        private readonly Uri _baseAddress;

        private HttpClientSettings(int maxAutomaticRedirections, bool useCookies, Uri baseAddress)
        {
            _maxRedirects = maxAutomaticRedirections;
            _useCookies = useCookies;
            _baseAddress = baseAddress;
        }

        /// <summary>
        /// Gets a <see cref="HttpClient"/> instance constructed by this <see cref="HttpClientSettings"/>.
        /// </summary>
        internal HttpClient Client
        {
            get
            {
                HttpClientHandler handler = new HttpClientHandler
                {
                    UseCookies = _useCookies,
                    AllowAutoRedirect = _maxRedirects > 0
                };
                if (_maxRedirects > 0)
                {
                    handler.MaxAutomaticRedirections = _maxRedirects;
                }

                return new HttpClient(handler, true)
                {
                    BaseAddress = _baseAddress,
                };
            }
        }

        public HttpClientSettings LimitAutomaticRedirectsTo(int maxRedirects)
        {
            return new HttpClientSettings(maxRedirects, _useCookies, _baseAddress);
        }

        public HttpClientSettings WithoutRedirects()
        {
            return new HttpClientSettings(0, _useCookies, _baseAddress);
        }

        public HttpClientSettings WithoutCookies()
        {
            return new HttpClientSettings(_maxRedirects, false, _baseAddress);
        }

        public HttpClientSettings WithCookies()
        {
            return new HttpClientSettings(_maxRedirects, true, _baseAddress);
        }

        public static HttpClientSettings WithoutBaseAddress()
        {
            return new HttpClientSettings(DefaultAutomaticRedirectLimit, true, null);
        }

        public static HttpClientSettings WithBaseAddress(Uri baseAddress)
        {
            return new HttpClientSettings(DefaultAutomaticRedirectLimit, true, baseAddress);
        }

        public static HttpClientSettings WithBaseAddress(string baseAddress)
        {
            return new HttpClientSettings(DefaultAutomaticRedirectLimit, true, new Uri(baseAddress));
        }
    }
}
