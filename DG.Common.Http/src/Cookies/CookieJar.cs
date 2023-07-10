using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;

namespace DG.Common.Http.Cookies
{
    public class CookieJar
    {
        private readonly ConcurrentDictionary<string, Cookie> _cookies = new ConcurrentDictionary<string, Cookie>();

        /// <summary>
        /// Updates the cookies contained in this <see cref="CookieJar"/> based on the given response.
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
                _cookies[cookie.Key] = cookie;
            }
        }
    }
}
