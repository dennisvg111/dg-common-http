using System.Net.Http;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Provides a custom constructor for the <see cref="HttpClient"/> class.
    /// </summary>
    public class HttpClientBuilder
    {
        private readonly Uulsid _cacheId = Uulsid.NewUulsid();
        internal string CacheId => _cacheId.ToString();

        /// <summary>
        /// Gets a <see cref="HttpClient"/> instance constructed by this <see cref="HttpClientBuilder"/>.
        /// </summary>
        public HttpClient Client
        {
            get
            {
                var client = _handler == null ? new HttpClient() : new HttpClient(_handler);

                return client;
            }
        }

        private HttpMessageHandler _handler;
    }
}
