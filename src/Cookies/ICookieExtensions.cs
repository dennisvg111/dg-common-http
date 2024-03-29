﻿using System;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// Provides extension methods for cookies.
    /// </summary>
    public static class ICookieExtensions
    {
        /// <summary>
        /// Indicates that the cookie is expired on the given <paramref name="date"/>.
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsExpiredOn(this ICookie cookie, DateTimeOffset date)
        {
            // MaxAge has precedence over Expires
            if (cookie.MaxAge.HasValue)
            {
                if (cookie.MaxAge.Value <= 0)
                {
                    return true;
                }
                return (date - cookie.ReceivedDate).TotalSeconds > cookie.MaxAge.Value;
            }

            return cookie.Expires.HasValue && date > cookie.Expires.Value;
        }

        /// <summary>
        /// Indicates that the cookie is expired, and thus should be removed.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool IsExpired(this ICookie cookie)
        {
            return cookie.IsExpiredOn(DateTimeOffset.UtcNow);
        }

        /// <summary>
        /// Indicates if this is a session cookie.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool IsSessionCookie(this ICookie cookie)
        {
            return !cookie.MaxAge.HasValue && !cookie.Expires.HasValue;
        }

        /// <summary>
        /// <para>Returns a value indicating if this cookie is valid according to <see cref="CookieRules.Default"/>.</para>
        /// <para>If this cookie is not valid, <paramref name="reason"/> will contain a reason explaining why this cookie is invalid.</para>
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static bool IsValid(this ICookie cookie, out string reason)
        {
            foreach (var rule in CookieRules.Default)
            {
                if (!rule.Check(cookie))
                {
                    reason = rule.Name;
                    return false;
                }
            }
            reason = string.Empty;
            return true;
        }

        /// <summary>
        /// Returns a value indicating if this cookie is valid according to <see cref="CookieRules.Default"/>.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool IsValid(this ICookie cookie)
        {
            return cookie.IsValid(out string _);
        }

        /// <summary>
        /// Generates a key based on name and path used by the <see cref="CookieJar"/> class to identify cookies when adding or replacing cookies.
        /// </summary>
        /// <returns></returns>
        public static string GenerateKey(this ICookie cookie)
        {
            var domain = string.IsNullOrEmpty(cookie.Domain) ? "*" + cookie.OriginUri.Host : cookie.Domain;
            var path = string.IsNullOrEmpty(cookie.Path) ? cookie.OriginUri.AbsolutePath : cookie.Path;
            if (path.Length == 0)
            {
                path = "/";
            }
            return $"{domain}{path}[{cookie.Name}]";
        }

        /// <summary>
        /// Returns a string that represents this cookie to be used in a <c>Cookie</c> header, in the format <c>name=value</c>.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string ToCookieHeaderString(this ICookie cookie)
        {
            return $"{cookie.Name}={cookie.Value}";
        }
    }
}
