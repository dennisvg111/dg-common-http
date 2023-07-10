using DG.Common.Http.Extensions;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace DG.Common.Http.Fluent
{
    public class FluentRequest
    {
        private readonly HttpMethod _method;
        private readonly Uri _uri;
        private readonly HttpContent _content;
        private readonly int _maxRedirects = HttpClientSettings.DefaultAutomaticRedirectLimit;

        /// <summary>
        /// The maximum number of redirects that this request will follow automatically.
        /// </summary>
        public int MaxRedirects => _maxRedirects;


        /// <summary>
        /// Gets a <see cref="HttpRequestMessage"/> instance constructed by this <see cref="FluentRequest"/>.
        /// </summary>
        public HttpRequestMessage Message
        {
            get
            {
                var message = new HttpRequestMessage(_method, _uri);
                return message;
            }
        }

        internal FluentRequest(HttpMethod method, Uri uri)
        {
            _method = method;
            _uri = uri;
        }

        private FluentRequest(HttpMethod method, Uri uri, HttpContent content, int maxRedirects) : this(method, uri)
        {
            _content = content;
            _maxRedirects = maxRedirects;
        }

        public FluentRequest WithContent(FluentFormContent content)
        {
            return new FluentRequest(_method, _uri, content.Content, _maxRedirects);
        }

        public FluentRequest WithSerializedJsonContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content);
            return WithJson(json);
        }

        public FluentRequest WithJson(string json)
        {
            var messageContent = new StringContent(json, Encoding.UTF8, "application/json");
            return new FluentRequest(_method, _uri, messageContent, _maxRedirects);
        }

        public FluentRequest LimitAutomaticRedirectsTo(int maxRedirects)
        {
            return new FluentRequest(_method, _uri, _content, maxRedirects);
        }

        public FluentRequest WithoutRedirects()
        {
            return new FluentRequest(_method, _uri, _content, 0);
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

        /// <summary>
        /// Creates a new instance of <see cref="FluentRequest"/> that describes the redirection by the given <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public FluentRequest RedirectForResponse(HttpResponseMessage response)
        {
            var changeToGet = response.IsGetRedirect(_method);
            var method = changeToGet ? HttpMethod.Get : _method;

            var location = response.Headers.Location;
            var solvedLocationUri = response.RequestMessage.RequestUri.CombineForRedirectLocation(location);

            var baseRedirect = new FluentRequest(method, solvedLocationUri, changeToGet ? null : _content, _maxRedirects - 1);

            return baseRedirect;
        }
    }
}
