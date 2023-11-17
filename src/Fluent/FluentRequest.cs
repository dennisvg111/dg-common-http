using DG.Common.Http.Authorization;
using DG.Common.Http.Cookies;
using DG.Common.Http.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Represents a request with methods that allow for chaining calls.
    /// </summary>
    public class FluentRequest
    {
        private readonly HttpMethod _method;
        private readonly Uri _uri;

        private HttpContent _content;
        private int _maxRedirects = HttpClientSettings.DefaultAutomaticRedirectLimit;
        private CookieJar _cookieJar;
        private CancellationToken _cancellationToken = CancellationToken.None;
        private HttpCompletionOption _completionOption = HttpCompletionOption.ResponseContentRead;
        private List<FluentHeader> _headers = new List<FluentHeader>();

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
        /// <para>Indicates if control is returned to the program immediately after the headers are read, or when the entire response is cached.</para>
        /// <para>Default is <see cref="HttpCompletionOption.ResponseContentRead"/>.</para>
        /// <para>Note that when this is set to <see cref="HttpCompletionOption.ResponseHeadersRead"/>, extra care should be taken to ensure the response is disposed.</para>
        /// </summary>
        public HttpCompletionOption CompletionOption => _completionOption;

        /// <summary>
        /// <para>The cancellation token to cancel this request.</para>
        /// <para>Default is <see cref="CancellationToken.None"/>.</para>
        /// </summary>
        public CancellationToken CancellationToken => _cancellationToken;

        /// <summary>
        /// A collection of headers that will be applied to this request.
        /// </summary>
        public IReadOnlyList<FluentHeader> Headers => _headers;

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
            var message = new HttpRequestMessage(_method, requestUri)
            {
                Content = _content
            };
            if (_cookieJar != null && requestUri.IsAbsoluteUri)
            {
                _cookieJar.ApplyTo(message);
            }
            foreach (var header in _headers.Where(h => h.ShouldApply))
            {
                if (header.IsContentHeader)
                {
                    message.Content?.Headers?.AddOrReplace(header);
                    continue;
                }
                message.Headers.AddOrReplace(header);
            }
            return message;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentRequest"/> with the given <see cref="HttpMethod"/> and <see cref="Uri"/>.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        internal FluentRequest(HttpMethod method, Uri uri)
        {
            _method = method;
            _uri = uri;
        }

        private void CopyTo(FluentRequest copy)
        {
            copy._content = _content;
            copy._maxRedirects = _maxRedirects;
            copy._cookieJar = _cookieJar;
            copy._cancellationToken = _cancellationToken;
            copy._completionOption = _completionOption;
            copy._headers = new List<FluentHeader>(_headers);
        }

        private FluentRequest Copy()
        {
            var copy = new FluentRequest(_method, _uri);
            CopyTo(copy);
            return copy;
        }

        /// <summary>
        /// Returns a copy of this request with the given <paramref name="header"/> added to the collection of headers.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public FluentRequest WithHeader(FluentHeader header)
        {
            var copy = Copy();
            copy._headers.Add(header);
            return copy;
        }

        /// <summary>
        /// Returns a copy of this request where the given <see cref="IAuthorizationHeaderProvider"/> is used to provide an <c>Authorizaton</c> header.
        /// </summary>
        /// <param name="headerProvider"></param>
        /// <returns></returns>
        public FluentRequest WithAuthorization(IAuthorizationHeaderProvider headerProvider)
        {
            return WithHeader(FluentHeader.Authorization(headerProvider));
        }

        /// <summary>
        /// Returns a copy of this request where the given <see cref="FluentAuthorization"/> is used to provide an <c>Authorizaton</c> header.
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public FluentRequest WithAuthorization(FluentAuthorization authorization)
        {
            return WithHeader(FluentHeader.Authorization(authorization));
        }

        /// <summary>
        /// Returns a copy of this request with the HTTP body and content headers set to the given <paramref name="content"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public FluentRequest WithContent(HttpContent content)
        {
            var copy = Copy();
            copy._content = content;
            return copy;
        }

        /// <summary>
        /// Returns a copy of this request with the HTTP body and content headers set to a <see cref="FormUrlEncodedContent"/> created using the given <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public FluentRequest WithContent(FluentFormContentBuilder builder)
        {
            return WithContent(builder.BuildUrlEncodedForm());
        }

        /// <summary>
        /// Returns a copy of this request with the HTTP body and content headers set to a <see cref="MultipartFormDataContent"/> created using the given <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public FluentRequest WithContent(FluentFormContentBuilder.IFluentMultipartFormContentBuilder builder)
        {
            return WithContent(builder.BuildMultipartForm());
        }

        /// <summary>
        /// Returns a copy of this request with the HTTP body and content headers set based on the JSON serialization of the given <paramref name="content"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public FluentRequest WithSerializedJsonContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content);
            return WithJsonContent(json);
        }

        /// <summary>
        /// Returns a copy of this request with the HTTP body and content headers set based on the given <paramref name="json"/> content.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public FluentRequest WithJsonContent(string json)
        {
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
            return WithContent(jsonContent);
        }

        /// <summary>
        /// Returns a copy of this request with the content set to an instance of <see cref="ByteArrayContent"/>, with the given bytes (optionally limited to only <paramref name="maxAmount"/> bytes.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="maxAmount"></param>
        /// <returns></returns>
        public FluentRequest WithByteArrayContent(byte[] bytes, int? maxAmount = null)
        {
            var content = new ByteArrayContent(bytes, 0, maxAmount ?? bytes.Length);

            var copy = Copy();
            copy._content = content;
            return copy;
        }

        /// <summary>
        /// Returns a copy of this request with the remaining allowed redirects set to <paramref name="maxRedirects"/>.
        /// </summary>
        /// <param name="maxRedirects"></param>
        /// <returns></returns>
        public FluentRequest LimitAutomaticRedirectsTo(int maxRedirects)
        {
            var copy = Copy();
            copy._maxRedirects = maxRedirects;
            return copy;
        }

        /// <summary>
        /// Returns a copy of this request that will not be automatically redirected.
        /// </summary>
        /// <returns></returns>
        public FluentRequest WithoutRedirects()
        {
            return LimitAutomaticRedirectsTo(0);
        }

        /// <summary>
        /// <para>Returns a copy of this request with the cookie jar set to the given <paramref name="cookieJar"/>.</para>
        /// <para>This means that any applicable cookies in this jar will be applied to this request, and any new cookies in the response will be saved to this jar.</para>
        /// </summary>
        /// <param name="cookieJar"></param>
        /// <returns></returns>
        public FluentRequest WithCookieJar(CookieJar cookieJar)
        {
            var copy = Copy();
            copy._cookieJar = cookieJar;
            return copy;
        }

        /// <summary>
        /// Returns a copy of this request with <see cref="CompletionOption"/> set to the given option.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public FluentRequest WithHttpCompletionOption(HttpCompletionOption option)
        {
            var copy = Copy();
            copy._completionOption = option;
            return copy;
        }

        /// <summary>
        /// Returns a copy of this request with <see cref="CancellationToken"/> set to the given token.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public FluentRequest WithCancellationToken(CancellationToken cancellationToken)
        {
            var copy = Copy();
            copy._cancellationToken = cancellationToken;
            return copy;
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

            var redirectedRequest = new FluentRequest(method, solvedLocationUri);
            CopyTo(redirectedRequest);

            redirectedRequest._content = changeToGet ? null : _content;
            redirectedRequest._maxRedirects = _maxRedirects - 1;

            return redirectedRequest;
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
