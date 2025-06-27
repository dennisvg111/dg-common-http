using System;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// Provides functionality for parsing cookies.
    /// </summary>
    public static class Cookie
    {
        /// <summary>
        /// Parses a HTTP Set-Cookie header value to an instance of <see cref="CookieWrapper"/>, and returns a value indicating if parsing succeeded.
        /// </summary>
        /// <param name="headerValue"></param>
        /// <param name="receivedDate"></param>
        /// <param name="originUri"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool TryParse(string headerValue, DateTimeOffset receivedDate, Uri originUri, out ICookie cookie)
        {
            bool success = ReadOnlyCookie.TryParse(headerValue, receivedDate, originUri, out ReadOnlyCookie readOnlyCookie);
            cookie = readOnlyCookie;
            return success;
        }
    }
}
