using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace DG.Common.Http.Fluent
{
    public class HttpRequestMessageBuilder
    {
        private readonly HttpMethod _method;
        private readonly UriBuilder _uriBuilder;
        private readonly HttpContent _content;

        /// <summary>
        /// Gets a <see cref="HttpRequestMessage"/> instance constructed by this <see cref="HttpRequestMessageBuilder"/>.
        /// </summary>
        public HttpRequestMessage Message
        {
            get
            {
                var message = new HttpRequestMessage(_method, _uriBuilder.Uri);
                return message;
            }
        }

        public HttpRequestMessageBuilder(HttpMethod method, UriBuilder uriBuilder)
        {
            _method = method;
            _uriBuilder = uriBuilder;
        }

        private HttpRequestMessageBuilder(HttpMethod method, UriBuilder uriBuilder, HttpContent content) : this(method, uriBuilder)
        {
            _content = content;
        }

        public HttpRequestMessageBuilder WithSerializedJsonContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content);
            return WithJson(json);
        }

        public HttpRequestMessageBuilder WithJson(string json)
        {
            var messageContent = new StringContent(json, Encoding.UTF8, "application/json");
            return new HttpRequestMessageBuilder(_method, _uriBuilder, messageContent);
        }

        public static HttpRequestMethod Get => HttpRequestMethod.For(HttpMethod.Get);
        public static HttpRequestMethod Post => HttpRequestMethod.For(HttpMethod.Post);
        public static HttpRequestMethod Put => HttpRequestMethod.For(HttpMethod.Put);
        public static HttpRequestMethod Delete => HttpRequestMethod.For(HttpMethod.Delete);

        /// <summary>
        /// This method allows for initializing a <see cref="HttpRequestMessageBuilder"/> with a non-default HTTP method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static HttpRequestMethod For(HttpMethod method)
        {
            return HttpRequestMethod.For(method);
        }
    }
}
