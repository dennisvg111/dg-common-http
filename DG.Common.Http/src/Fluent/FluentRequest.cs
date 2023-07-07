using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace DG.Common.Http.Fluent
{
    public class FluentRequest
    {
        private readonly HttpMethod _method;
        private readonly UriBuilder _uriBuilder;
        private readonly HttpContent _content;

        /// <summary>
        /// Gets a <see cref="HttpRequestMessage"/> instance constructed by this <see cref="FluentRequest"/>.
        /// </summary>
        public HttpRequestMessage Message
        {
            get
            {
                var message = new HttpRequestMessage(_method, _uriBuilder.Uri);
                return message;
            }
        }

        internal FluentRequest(HttpMethod method, UriBuilder uriBuilder)
        {
            _method = method;
            _uriBuilder = uriBuilder;
        }

        private FluentRequest(HttpMethod method, UriBuilder uriBuilder, HttpContent content) : this(method, uriBuilder)
        {
            _content = content;
        }

        public FluentRequest WithSerializedJsonContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content);
            return WithJson(json);
        }

        public FluentRequest WithJson(string json)
        {
            var messageContent = new StringContent(json, Encoding.UTF8, "application/json");
            return new FluentRequest(_method, _uriBuilder, messageContent);
        }

        /// <summary>
        /// The GET method requests a representation of the specified resource. Requests using GET should only retrieve data.
        /// </summary>
        public static FluentRequestMethod Get => FluentRequestMethod.For(HttpMethod.Get);

        /// <summary>
        /// The HEAD method asks for a response identical to a <see cref="Get"/> request, but without the response body.
        /// </summary>
        public static FluentRequestMethod Head => FluentRequestMethod.For(HttpMethod.Head);

        /// <summary>
        /// The POST method submits an entity to the specified resource, often causing a change in state or side effects on the server.
        /// </summary>
        public static FluentRequestMethod Post => FluentRequestMethod.For(HttpMethod.Post);

        /// <summary>
        /// The PUT method replaces all current representations of the target resource with the request payload.
        /// </summary>
        public static FluentRequestMethod Put => FluentRequestMethod.For(HttpMethod.Put);

        /// <summary>
        /// The DELETE method deletes the specified resource.
        /// </summary>
        public static FluentRequestMethod Delete => FluentRequestMethod.For(HttpMethod.Delete);

        /// <summary>
        /// The PATCH method applies partial modifications to a resource.
        /// </summary>
        public static FluentRequestMethod Patch => FluentRequestMethod.For(new HttpMethod("PATCH"));

        /// <summary>
        /// This function allows for initializing a <see cref="FluentRequest"/> with a non-default HTTP method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static FluentRequestMethod For(HttpMethod method)
        {
            return FluentRequestMethod.For(method);
        }
    }
}
