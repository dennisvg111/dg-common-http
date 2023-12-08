using System;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// Provides extension methods for cookies.
    /// </summary>
    public static class CookieExtensions
    {
        /// <summary>
        /// Indicates that the cookie is expired, and thus should be removed.
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsExpiredOn(this ICookieIngredients cookie, DateTimeOffset date)
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
    }
}
