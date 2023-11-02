using DG.Common.Http.Authorization;
using DG.Common.Http.Cookies;
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
        private readonly CookieJar _cookieJar;
        private readonly IAuthorizationHeaderProvider _authorizationProvider;

        /// <summary>
        /// The maximum number of redirects that this request will follow automatically.
        /// </summary>
        public int MaxRedirects => _maxRedirects;

        /// <summary>
        /// <para>Gets a <see cref="HttpRequestMessage"/> instance constructed by this <see cref="FluentRequest"/>.</para>
        /// <para>Note that it is recommended to send a fluent request directly using <see cref="HttpClientExtensions.SendAsync(HttpClient, FluentRequest)"/>.</para>
        /// </summary>
        public HttpRequestMessage Message => MessageForBaseUri(null);

        /// <summary>
        /// <para>Gets a <see cref="HttpRequestMessage"/> instance constructed by this <see cref="FluentRequest"/>.</para>
        /// <para>Note that it is recommended to send a fluent request directly using <see cref="HttpClientExtensions.SendAsync(HttpClient, FluentRequest)"/>.</para>
        /// </summary>
        /// <param name="baseUri">The base <see cref="Uri"/> for this request. This will be used for things like determining which cookies should be applied to this request message.</param>
        /// <returns></returns>
        public HttpRequestMessage MessageForBaseUri(Uri baseUri)
        {
            var requestUri = _uri;
            if (baseUri != null)
            {
                requestUri = new Uri(baseUri, _uri);
            }
            var message = new HttpRequestMessage(_method, requestUri);
            message.Content = _content;
            if (_cookieJar != null && requestUri.IsAbsoluteUri)
            {
                _cookieJar.ApplyTo(message);
            }
            if (_authorizationProvider != null && _authorizationProvider.IsAuthorized)
            {
                _authorizationProvider.TryDecorateMessage(message);
            }
            return message;
        }

        internal FluentRequest(HttpMethod method, Uri uri)
        {
            _method = method;
            _uri = uri;
        }

        private FluentRequest(HttpMethod method, Uri uri, HttpContent content, int maxRedirects, CookieJar cookieJar, IAuthorizationHeaderProvider authorization) : this(method, uri)
        {
            _content = content;
            _maxRedirects = maxRedirects;
            _cookieJar = cookieJar;
            _authorizationProvider = authorization;
        }

        public FluentRequest WithContent(HttpContent content)
        {
            return new FluentRequest(_method, _uri, content, _maxRedirects, _cookieJar, _authorizationProvider);
        }

        public FluentRequest WithContent(FluentFormContent content)
        {
            return new FluentRequest(_method, _uri, content.Content, _maxRedirects, _cookieJar, _authorizationProvider);
        }

        public FluentRequest WithSerializedJsonContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content);
            return WithJson(json);
        }

        public FluentRequest WithJson(string json)
        {
            var messageContent = new StringContent(json, Encoding.UTF8, "application/json");
            return new FluentRequest(_method, _uri, messageContent, _maxRedirects, _cookieJar, _authorizationProvider);
        }

        public FluentRequest LimitAutomaticRedirectsTo(int maxRedirects)
        {
            return new FluentRequest(_method, _uri, _content, maxRedirects, _cookieJar, _authorizationProvider);
        }

        public FluentRequest WithoutRedirects()
        {
            return new FluentRequest(_method, _uri, _content, 0, _cookieJar, _authorizationProvider);
        }

        public FluentRequest WithCookieJar(CookieJar cookieJar)
        {
            return new FluentRequest(_method, _uri, _content, _maxRedirects, cookieJar, _authorizationProvider);
        }

        /// <summary>
        /// Returns a copy of this request with an <see cref="ConstantAuthorizationHeaderProvider"/> set based on <paramref name="headerValue"/>.
        /// </summary>
        /// <param name="headerValue"></param>
        /// <returns></returns>
        public FluentRequest WithAuthorizationHeader(string headerValue)
        {
            var headerProvider = new ConstantAuthorizationHeaderProvider(headerValue);
            return WithAuthorizationHeaderProvider(headerProvider);
        }

        /// <summary>
        /// Returns a copy of this request with <paramref name="headerProvider"/> set to provide the <c>Authorization</c> header.
        /// </summary>
        /// <param name="headerProvider"></param>
        /// <returns></returns>
        public FluentRequest WithAuthorizationHeaderProvider(IAuthorizationHeaderProvider headerProvider)
        {
            return new FluentRequest(_method, _uri, _content, _maxRedirects, _cookieJar, headerProvider);
        }

        /// <summary>
        /// Returns a copy of this request with an <see cref="ExpiringAuthorizationHeaderProvider"/> set based on <paramref name="authorizationProvider"/>.
        /// </summary>
        /// <param name="authorizationProvider"></param>
        /// <returns></returns>
        public FluentRequest WithAuthorizationHeaderProvider(IExpiringAuthorizationProvider authorizationProvider)
        {
            var headerProvider = new ExpiringAuthorizationHeaderProvider(authorizationProvider);
            return WithAuthorizationHeaderProvider(headerProvider);
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
        /// <para>Creates a new instance of <see cref="FluentRequest"/> that describes the redirection by the given <see cref="HttpResponseMessage"/>.</para>
        /// <para>Note that the value of <see cref="MaxRedirects"/> will always be exactly one less than the original request.</para>
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public FluentRequest RedirectForResponse(HttpResponseMessage response)
        {
            //TODO: optionally set origin uri header

            var changeToGet = response.IsGetRedirect(_method);
            var method = changeToGet ? HttpMethod.Get : _method;

            var location = response.Headers.Location;
            var solvedLocationUri = response.RequestMessage.RequestUri.CombineForRedirectLocation(location);

            var baseRedirect = new FluentRequest(method, solvedLocationUri, changeToGet ? null : _content, _maxRedirects - 1, _cookieJar, _authorizationProvider);

            return baseRedirect;
        }

        internal void CollectCookiesIfNeeded(HttpResponseMessage response)
        {
            if (_cookieJar == null)
            {
                return;
            }
            _cookieJar.CollectFrom(response);
        }
    }
}
