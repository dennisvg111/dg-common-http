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

        /// <summary>
        /// The default number of milliseconds before the request times out.
        /// </summary>
        public readonly static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(100);

        private readonly Uulsid _cacheId = Uulsid.NewUulsid();
        internal string CacheId => _cacheId.ToString();

        private readonly int _maxRedirects;
        private readonly bool _useCookies;
        private readonly Uri _baseAddress;
        private readonly TimeSpan _maxTimeout;

        private HttpClientSettings(Uri baseAddress)
        {
            _baseAddress = baseAddress;
            _maxRedirects = DefaultAutomaticRedirectLimit;
            _maxTimeout = TimeSpan.FromSeconds(100);
        }

        private HttpClientSettings(int maxAutomaticRedirections, bool useCookies, Uri baseAddress, TimeSpan maxTimeout) : this(baseAddress)
        {
            _maxRedirects = maxAutomaticRedirections;
            _useCookies = useCookies;
            _maxTimeout = maxTimeout;
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
                    Timeout = _maxTimeout
                };
            }
        }

        public HttpClientSettings LimitAutomaticRedirectsTo(int maxRedirects)
        {
            return new HttpClientSettings(maxRedirects, _useCookies, _baseAddress, _maxTimeout);
        }

        public HttpClientSettings WithoutRedirects()
        {
            return new HttpClientSettings(0, _useCookies, _baseAddress, _maxTimeout);
        }

        public HttpClientSettings WithoutCookies()
        {
            return new HttpClientSettings(_maxRedirects, false, _baseAddress, _maxTimeout);
        }

        public HttpClientSettings WithCookies()
        {
            return new HttpClientSettings(_maxRedirects, true, _baseAddress, _maxTimeout);
        }

        public HttpClientSettings WithTimeout(TimeSpan timeout)
        {
            return new HttpClientSettings(_maxRedirects, true, _baseAddress, timeout);
        }

        public static HttpClientSettings WithoutBaseAddress()
        {
            return new HttpClientSettings(null);
        }

        public static HttpClientSettings WithBaseAddress(Uri baseAddress)
        {
            return new HttpClientSettings(baseAddress);
        }

        public static HttpClientSettings WithBaseAddress(string baseAddress)
        {
            return new HttpClientSettings(new Uri(baseAddress));
        }
    }
}
