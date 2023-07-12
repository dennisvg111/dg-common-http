using DG.Common.Exceptions;
using DG.Common.Http.Extensions;
using System;

namespace DG.Common.Http.Cookies
{
    public class Cookie : IComparable<Cookie>
    {
        private readonly IRawCookie _base;
        private readonly CookiePath _path;
        private readonly CookieExpiration _expiration;

        private readonly bool _isMarkedHost;
        private readonly bool _isMarkedSecure;

        /// <inheritdoc cref="IRawCookie.Name"/>
        public string Name => _base.Name;

        /// <inheritdoc cref="IRawCookie.Value"/>
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
        public Cookie(IRawCookie cookieBase)
        {
            ThrowIf.Parameter.IsNull(cookieBase, nameof(cookieBase));
            ThrowIf.Parameter.IsNull(cookieBase.OriginUri, nameof(cookieBase.OriginUri));
            ThrowIf.Parameter.Matches(cookieBase.OriginUri, (uri) => !uri.IsAbsoluteUri, nameof(cookieBase.OriginUri), "Parameter must be an absolute URI.");
            ThrowIf.Parameter.IsNullOrEmpty(cookieBase.Name, nameof(cookieBase.Name));

            _base = cookieBase;
            _path = new CookiePath(cookieBase);
            _expiration = new CookieExpiration(cookieBase);

            _isMarkedHost = cookieBase.Name.StartsWith("__Host-", StringComparison.Ordinal);
            _isMarkedSecure = cookieBase.Name.StartsWith("__Secure-", StringComparison.Ordinal);
        }

        public bool AppliesTo(Uri requestUri)
        {
            if (_base.IsSecure && !requestUri.IsSecure())
            {
                return false;
            }
            return !IsExpired() && _path.IsMatch(requestUri);
        }

        /// <summary>
        /// Indicates that the cookie is expired, and thus should be removed.
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return _expiration.IsExpiredOn(DateTimeOffset.UtcNow);
        }

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
            if (!RawHeaderCookie.TryParse(headerValue, receievedDate, originUri, out RawHeaderCookie rawCookie))
            {
                cookie = null;
                return false;
            }
            cookie = new Cookie(rawCookie);
            return true;
        }
    }
}
