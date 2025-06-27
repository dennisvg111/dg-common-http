using DG.Common.Http.Fluent;
using DG.Common.Http.Headers;
using System;
using System.Globalization;
using System.Linq;

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
            if (!headerValue.Contains("="))
            {
                cookie = null;
                return false;
            }
            var properties = HeaderValuePart.ParseList(headerValue);
            if (properties == null || properties.Length == 0 || (properties[0].Name?.Length ?? 0) == 0)
            {
                cookie = null;
                return false;
            }
            FluentCookie fluentCookie = new FluentCookie(properties[0].Name, properties[0].Value, originUri, receivedDate);

            properties = properties.Skip(1).ToArray();
            cookie = ParseAdditionalProperties(fluentCookie, properties);
            return true;
        }

        private static FluentCookie ParseAdditionalProperties(FluentCookie cookie, HeaderValuePart[] properties)
        {
            if (properties.TryGet("Domain", out string domain))
            {
                cookie = cookie.WithDomain(domain);
            }
            if (properties.TryGet("Path", out string path))
            {
                cookie = cookie.WithPath(path);
            }

            cookie = cookie
                .WithHttpOnlyValue(properties.TryGet("HttpOnly", out _))
                .WithSecureValue(properties.TryGet("Secure", out _));

            if (properties.TryGet("SameSite", out string sameSiteString) && Enum.TryParse(sameSiteString, true, out SameSitePolicy policy))
            {
                cookie = cookie.WithSameSitePolicy(policy);
            }

            if (properties.TryGet("Expires", out string expiresString) && DateTimeOffset.TryParse(expiresString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset expires))
            {
                cookie = cookie.WithExpiration(expires);
            }
            if (properties.TryGet("Max-Age", out string maxAgeString) && int.TryParse(maxAgeString, out int maxAge))
            {
                cookie = cookie.WithMaxAge(maxAge);
            }
            return cookie;
        }
    }
}
