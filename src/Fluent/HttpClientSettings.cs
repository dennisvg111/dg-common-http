using System;
using System.Net.Http;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Provides a custom constructor for the <see cref="HttpClient"/> class.
    /// </summary>
    public sealed class HttpClientSettings
    {
        /// <summary>
        /// The default number of milliseconds before the request times out.
        /// </summary>
        public readonly static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(100);\
        internal string CacheId => GetHashCode().ToString();

        private readonly Uri _baseAddress;
        private readonly TimeSpan _maxTimeout;

        private HttpClientSettings(Uri baseAddress)
        {
            _baseAddress = baseAddress;
            _maxTimeout = TimeSpan.FromSeconds(100);
        }

        private HttpClientSettings(Uri baseAddress, TimeSpan maxTimeout) : this(baseAddress)
        {
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
                    UseCookies = false,
                    AllowAutoRedirect = false
                };

                return new HttpClient(handler, true)
                {
                    BaseAddress = _baseAddress,
                    Timeout = _maxTimeout
                };
            }
        }

        public HttpClientSettings WithTimeout(TimeSpan timeout)
        {
            return new HttpClientSettings(_baseAddress, timeout);
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

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode
                .Of(_baseAddress)
                .And(_maxTimeout);
        }
    }
}
