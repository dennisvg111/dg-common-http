using System;
using System.Net.Http;

namespace DG.Common.Http.FluentBuilders
{
    public class HttpRequestMessageBuilder
    {
        private readonly HttpMethod _method;
        private readonly UriBuilder _uriBuilder;

        public HttpRequestMessage Message
        {
            get
            {
                return new HttpRequestMessage(_method, _uriBuilder.Uri);
            }
        }

        private HttpRequestMessageBuilder(HttpMethod method, UriBuilder uriBuilder)
        {
            _method = method;
            _uriBuilder = uriBuilder;
        }

        public static HttpRequestMessageBuilder ForGet(string url)
        {
            return new HttpRequestMessageBuilder(HttpMethod.Get, new UriBuilder(url));
        }

        public static HttpRequestMessageBuilder ForPost(string url)
        {
            return new HttpRequestMessageBuilder(HttpMethod.Get, new UriBuilder(url));
        }
    }
}
