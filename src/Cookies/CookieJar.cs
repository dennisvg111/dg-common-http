﻿using DG.Common.Http.Extensions;
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
        private readonly static CookieComparer _cookieComparer = new CookieComparer();

        private readonly ConcurrentDictionary<string, Cookie> _cookies = new ConcurrentDictionary<string, Cookie>();

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
                if (!Cookie.TryParse(headerValue, receivedDate, requestUri, out Cookie cookie))
                {
                    continue;
                }
                string key = cookie.GenerateKey();
                if (cookie.IsExpired())
                {
                    _cookies.TryRemove(key, out Cookie _);
                    continue;
                }
                _cookies[key] = cookie;
            }
        }

        /// <summary>
        /// Adds all relevant cookies from this <see cref="CookieJar"/> to the given request as cookie headers.
        /// </summary>
        /// <param name="request"></param>
        public void ApplyTo(HttpRequestMessage request)
        {
            var uri = request.RequestUri;
            var cookieBuilder = new StringBuilder();
            if (request.Headers.TryGetValues("Cookie", out IEnumerable<string> values) && values.Any())
            {
                cookieBuilder.Append(values.First());
            }
            bool replaceNeeded = false;
            foreach (ICookieIngredients cookie in _cookies.Values.OrderBy(c => c, _cookieComparer))
            {
                if (!cookie.AppliesTo(uri))
                {
                    continue;
                }
                replaceNeeded = true;
                if (cookieBuilder.Length > 0)
                {
                    cookieBuilder.Append("; ");
                }
                cookieBuilder.Append(cookie.ToCookieHeaderString());
            }
            if (replaceNeeded)
            {
                request.Headers.AddOrReplace("Cookie", cookieBuilder.ToString());
            }
        }
    }
}
