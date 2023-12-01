using System;
using System.Net.Http;

namespace DG.Common.Http
{
    /// <summary>
    /// Provides a custom constructor for the <see cref="HttpClient"/> class.
    /// </summary>
    public sealed class HttpClientSettings
    {
        /// <summary>
        /// The default maximum number of redirects that a request will follow automatically.
        /// </summary>
        public const int DefaultAutomaticRedirectLimit = 50;

        /// <summary>
        /// The default number of seconds (100) before the request times out.
        /// </summary>
        public readonly static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(100);

        private readonly Uri _baseAddress;

        private int _maxRedirects;
        private bool _useCookies;
        private TimeSpan _maxTimeout;

        internal Uri BaseAddress => _baseAddress;
        internal int MaxRedirects => _maxRedirects;
        internal TimeSpan MaxTimeout => _maxTimeout;
        internal bool UseCookies => _useCookies;

        private HttpClientSettings(Uri baseAddress)
        {
            _baseAddress = baseAddress;
            _maxRedirects = DefaultAutomaticRedirectLimit;
            _maxTimeout = TimeSpan.FromSeconds(100);
        }

        private void CopyTo(HttpClientSettings copy)
        {
            copy._maxRedirects = _maxRedirects;
            copy._useCookies = _useCookies;
            copy._maxTimeout = _maxTimeout;
        }

        private HttpClientSettings Copy()
        {
            var copy = new HttpClientSettings(_baseAddress);
            CopyTo(copy);
            return copy;
        }

        /// <summary>
        /// Returns a copy of this <see cref="HttpClientSettings"/> where the maximum amount of automatic redirects is limited to <paramref name="maxRedirects"/>.
        /// </summary>
        /// <param name="maxRedirects"></param>
        /// <returns></returns>
        public HttpClientSettings LimitAutomaticRedirectsTo(int maxRedirects)
        {
            var copy = Copy();
            copy._maxRedirects = maxRedirects;
            return copy;
        }

        /// <summary>
        /// Returns a copy of this <see cref="HttpClientSettings"/> where redirects will not be automatically followed.
        /// </summary>
        /// <returns></returns>
        public HttpClientSettings WithoutRedirects()
        {
            return LimitAutomaticRedirectsTo(0);
        }

        /// <summary>
        /// Returns a copy of this <see cref="HttpClientSettings"/> where <see cref="HttpClientHandler.CookieContainer"/> will be used to automatically apply and retrieve cookies.
        /// </summary>
        /// <returns></returns>
        public HttpClientSettings WithoutCookies()
        {
            var copy = Copy();
            copy._useCookies = false;
            return copy;
        }

        /// <summary>
        /// Returns a copy of this <see cref="HttpClientSettings"/> where <see cref="HttpClientHandler.CookieContainer"/> will NOT be used to automatically apply and retrieve cookies.
        /// </summary>
        /// <returns></returns>
        public HttpClientSettings WithCookies()
        {
            var copy = Copy();
            copy._useCookies = true;
            return copy;
        }

        /// <summary>
        /// Returns a copy of this <see cref="HttpClientSettings"/> where requests will timeout after <paramref name="timeout"/>.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public HttpClientSettings WithTimeout(TimeSpan timeout)
        {
            var copy = Copy();
            copy._maxTimeout = timeout;
            return copy;
        }

        /// <summary>
        /// Creates a new instance of <see cref="HttpClientSettings"/> without a base address.
        /// </summary>
        /// <returns></returns>
        public static HttpClientSettings WithoutBaseAddress()
        {
            return new HttpClientSettings(null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="HttpClientSettings"/> with the given <paramref name="baseAddress"/>.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        public static HttpClientSettings WithBaseAddress(Uri baseAddress)
        {
            return new HttpClientSettings(baseAddress);
        }

        /// <summary>
        /// Creates a new instance of <see cref="HttpClientSettings"/> with the given <paramref name="baseAddress"/>.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        public static HttpClientSettings WithBaseAddress(string baseAddress)
        {
            return new HttpClientSettings(new Uri(baseAddress));
        }

        /// <summary>
        /// Returns a string to be used as a cache id. If two instances of <see cref="HttpClientSettings"/> return the same string, it is assumed they may result in the same <see cref="HttpClient"/>.
        /// </summary>
        /// <returns></returns>
        internal string GetCacheId()
        {
            return $"{_baseAddress} - {_maxTimeout},{_maxRedirects},{_useCookies}";
        }
    }
}
