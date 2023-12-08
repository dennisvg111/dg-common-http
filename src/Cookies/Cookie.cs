using DG.Common.Exceptions;
using DG.Common.Http.Extensions;
using System;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// Represents a cookie.
    /// </summary>
    public class Cookie : IComparable<Cookie>
    {
        private readonly ICookieIngredients _base;
        private readonly CookiePath _path;

        /// <inheritdoc cref="ICookieIngredients.Name"/>
        public string Name => _base.Name;

        /// <inheritdoc cref="ICookieIngredients.Value"/>
        public string Value => _base.Value;

        /// <summary>
        /// Indicates if this is a session cookie.
        /// </summary>
        public bool IsSessionCookie => !_expiration.HasExpiration;

        /// <summary>
        /// Generates a key based on name and path used by the <see cref="CookieJar"/> to identify cookies when adding or replacing cookies.
        /// </summary>
        /// <returns></returns>
        public string GenerateKey()
        {
            var domain = string.IsNullOrEmpty(_base.Domain) ? "*" + _base.OriginUri.Host : _base.Domain;
            var path = string.IsNullOrEmpty(_base.Path) ? _base.OriginUri.AbsolutePath : _base.Path;
            if (path.Length == 0)
            {
                path = "/";
            }
            return $"{domain}{path}[{_base.Name}]";
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Cookie"/> using the given cookie properties.
        /// </summary>
        /// <param name="cookieBase"></param>
        public Cookie(ICookieIngredients cookieBase)
        {
            ThrowIf.Parameter.IsNull(cookieBase, nameof(cookieBase));
            ThrowIf.Parameter.IsNull(cookieBase.OriginUri, nameof(cookieBase.OriginUri));
            ThrowIf.Parameter.Matches(cookieBase.OriginUri, (uri) => !uri.IsAbsoluteUri, nameof(cookieBase.OriginUri), "Parameter must be an absolute URI.");
            ThrowIf.Parameter.IsNullOrEmpty(cookieBase.Name, nameof(cookieBase.Name));

            _base = cookieBase;
            _path = new CookiePath(cookieBase);
        }

        /// <summary>
        /// Returns a value indicating if this cookies applies to the given <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public bool AppliesTo(Uri requestUri)
        {
            if (_base.IsSecure && !requestUri.IsSecure())
            {
                return false;
            }
            return !_base.IsExpiredOn(DateTimeOffset.UtcNow) && _path.IsMatch(requestUri);
        }

        /// <summary>
        /// Indicates if this cookie is valid. If not <paramref name="reason"/> will contain the reason this cookie is invalid.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool IsValid(out string reason)
        {
            foreach (var rule in CookieRules.Default)
            {
                if (!rule.Check(_base))
                {
                    reason = rule.Name;
                    return false;
                }
            }
            reason = string.Empty;
            return true;
        }

        /// <summary>
        /// Indicates if this cookie is valid according to the given <see cref="ValidityRule"/>.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool IsValidAccordingTo(ValidityRule rule)
        {
            return rule.Check(_base);
        }

        /// <summary>
        /// Indicates that the cookie is expired, and thus should be removed.
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return _base.IsExpiredOn(DateTimeOffset.UtcNow);
        }

        /// <summary>
        /// Compares two cookies, and returns a value if this cookies path is more specific or received before the other cookie.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Cookie other)
        {
            int pathComparison = (_base.Path?.Length ?? 0).CompareTo(other._base.Path?.Length ?? 0);
            if (pathComparison != 0)
            {
                //longest first.
                return -pathComparison;
            }
            return _base.ReceivedDate.CompareTo(other._base.ReceivedDate);
        }

        /// <summary>
        /// Converts this cookie to a string representation in the format <c>name=value</c>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{_base.Name}={_base.Value}";
        }

        /// <summary>
        /// Parses a HTTP Set-Cookie header value to an instance of <see cref="Cookie"/>, and returns a value indicating if parsing succeeded.
        /// </summary>
        /// <param name="headerValue"></param>
        /// <param name="receievedDate"></param>
        /// <param name="originUri"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool TryParse(string headerValue, DateTimeOffset receievedDate, Uri originUri, out Cookie cookie)
        {
            if (!SetCookieHeaderIngredients.TryParse(headerValue, receievedDate, originUri, out SetCookieHeaderIngredients rawCookie))
            {
                cookie = null;
                return false;
            }
            cookie = new Cookie(rawCookie);
            return true;
        }
    }
}
