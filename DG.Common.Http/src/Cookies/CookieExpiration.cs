using System;

namespace DG.Common.Http.Cookies
{
    internal class CookieExpiration
    {
        private readonly bool _instantExpiration;
        private readonly DateTimeOffset? _expirationDate;

        public CookieExpiration(IRawCookie cookieDough)
        {
            if (cookieDough.MaxAge.HasValue)
            {
                if (cookieDough.MaxAge.Value <= 0)
                {
                    _instantExpiration = true;
                }
                _expirationDate = cookieDough.ReceivedDate + TimeSpan.FromSeconds(cookieDough.MaxAge.Value);
            }
            else
            {
                _expirationDate = cookieDough.Expires;
            }
        }

        /// <summary>
        /// Indicates that the cookie is expired, and thus should be removed.
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            if (_instantExpiration)
            {
                return true;
            }
            if (!_expirationDate.HasValue)
            {
                return false;
            }
            return DateTimeOffset.UtcNow > _expirationDate.Value;
        }
    }
}
