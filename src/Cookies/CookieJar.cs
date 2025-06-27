using DG.Common.Http.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// A container for cookies that provides functionality for collecting cookies from a <see cref="HttpResponseMessage"/>, and applying cookies to a <see cref="HttpRequestMessage"/>.
    /// </summary>
    public class CookieJar
    {
        private readonly static ICookieComparer _cookieComparer = new ICookieComparer();

        private readonly bool _rejectInvalidCookies = true;
        private readonly ConcurrentDictionary<string, CookieWrapper> _cookies = new ConcurrentDictionary<string, CookieWrapper>();

        /// <summary>
        /// <para>Initializes a new instance of <see cref="CookieJar"/>.</para>
        /// <para>If <paramref name="rejectInvalidCookies"/> is <see langword="true"/>, cookies will only be added to this jar during <see cref="CollectFrom(HttpResponseMessage)"/> if they are valid according to <see cref="CookieRules.Default"/>.</para>
        /// </summary>
        /// <param name="rejectInvalidCookies"></param>
        public CookieJar(bool rejectInvalidCookies = true)
        {
            _rejectInvalidCookies = rejectInvalidCookies;
        }

        /// <summary>
        /// <para>Tries to add the given <see cref="ICookie"/> to this <see cref="CookieJar"/>, and returns a value indicating if this action succeeded.</para>
        /// <para>If this function returns <see langword="false"/>, this can mean the cookie is invalid or expired.</para>
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public bool TryAdd(ICookie cookie)
        {
            if (_rejectInvalidCookies && !cookie.IsValid())
            {
                return false;
            }
            string key = cookie.GenerateKey();
            if (cookie.IsExpired())
            {
                _cookies.TryRemove(key, out CookieWrapper _);
                return false;
            }
            _cookies[key] = new CookieWrapper(cookie);
            return true;
        }

        /// <summary>
        /// Updates the cookies contained in this <see cref="CookieJar"/> based on the <c>Set-Cookie</c> header given response.
        /// </summary>
        /// <param name="response"></param>
        public void CollectFrom(HttpResponseMessage response)
        {
            var receivedDate = DateTimeOffset.UtcNow;
            if (!response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> cookieHeaderValues))
            {
                return;
            }
            var requestUri = response.RequestMessage.RequestUri;

            foreach (var headerValue in cookieHeaderValues)
            {
                if (!Cookie.TryParse(headerValue, receivedDate, requestUri, out ICookie cookie))
                {
                    continue;
                }
                TryAdd(cookie);
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that iterates over the cookies in this jar that apply to the give <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public IEnumerable<ICookie> GetFor(Uri uri)
        {
            foreach (var wrapper in _cookies.Values.OrderBy(c => c.Cookie, _cookieComparer))
            {
                if (!wrapper.AppliesTo(uri))
                {
                    continue;
                }
                yield return wrapper.Cookie;
            }
        }

        /// <summary>
        /// Adds all relevant cookies from this <see cref="CookieJar"/> to the given request as cookie headers.
        /// </summary>
        /// <param name="request"></param>
        public void ApplyTo(HttpRequestMessage request)
        {
            var uri = request.RequestUri;
            var requestCookies = GetFor(uri);
            if (!requestCookies.Any())
            {
                return;
            }

            var cookieBuilder = new StringBuilder();
            if (request.Headers.TryGetValues("Cookie", out IEnumerable<string> values) && values.Any())
            {
                cookieBuilder.Append(values.First());
            }

            foreach (var cookie in requestCookies)
            {
                if (cookieBuilder.Length > 0)
                {
                    cookieBuilder.Append("; ");
                }
                cookieBuilder.Append(cookie.ToCookieHeaderString());
            }

            request.Headers.AddOrReplace("Cookie", cookieBuilder.ToString());
        }
    }
}
