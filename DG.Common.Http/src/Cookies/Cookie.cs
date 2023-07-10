using DG.Common.Http.Extensions;
using DG.Common.Http.Headers;
using System;
using System.Linq;

namespace DG.Common.Http.Cookies
{
    public class Cookie
    {
        private readonly string _name;
        private readonly bool _isHostCookie;
        private readonly bool _isSecureCookie;

        private readonly string _value;
        private readonly Uri _originUri;

        private string _domain;
        private string _path;

        private readonly DateTimeOffset _receivedDate;
        private DateTimeOffset? _expires;
        private int? _maxAge; //in seconds

        private bool _secure;
        private bool _httpOnly;
        private SameSite _samesite = SameSite.Lax;

        public string Key
        {
            get
            {
                var domain = string.IsNullOrEmpty(_domain) ? "*." + _originUri.Host : _domain;
                var path = string.IsNullOrEmpty(_path) ? _originUri.AbsolutePath : _path;
                if (path.Length == 0)
                {
                    path = "/";
                }
                return $"{domain}{path}[{_name}]";
            }
        }

        private Cookie(string name, string value, Uri originUri, DateTimeOffset receivedDate)
        {
            _name = name;
            _isHostCookie = _name.StartsWith("__Host-", StringComparison.Ordinal);
            _isSecureCookie = _name.StartsWith("__Secure-", StringComparison.Ordinal);

            _value = value;
            _originUri = originUri;
            _receivedDate = receivedDate;
        }

        public CookieValidity GetValidity()
        {
            if (_originUri == null || !_originUri.IsAbsoluteUri)
            {
                return CookieValidity.MisformedOriginUri;
            }
            if (_secure && !_originUri.IsSecure())
            {
                return CookieValidity.OriginUriMustBeSecure;
            }

            if (!string.IsNullOrEmpty(_domain))
            {

            }

            return CookieValidity.Valid;
        }

        public bool IsExpired()
        {
            if (_maxAge.HasValue)
            {
                if (_maxAge <= 0)
                {
                    return true;
                }
                return DateTimeOffset.UtcNow - _receivedDate > TimeSpan.FromSeconds(_maxAge.Value);
            }
            if (_expires.HasValue)
            {
                return DateTimeOffset.UtcNow > _expires.Value;
            }
            return false;
        }

        public bool AppliesTo(Uri requestUri)
        {
            if (_secure && !requestUri.IsSecure())
            {
                return false;
            }
            return GetValidity() == CookieValidity.Valid &&
                !IsExpired()
                && IsDomainMatch(requestUri)
                && IsPathMatch(requestUri);
        }

        private bool IsDomainMatch(Uri requestUri)
        {
            if (string.IsNullOrEmpty(_domain))
            {
                return _originUri.Host.Equals(_originUri.Host, StringComparison.OrdinalIgnoreCase);
            }

            var trimmedDomain = _domain.StartsWith(".", StringComparison.Ordinal) ? _domain.Substring(1) : _domain;
            if (_originUri.Host.Equals(trimmedDomain, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (_originUri.Host.EndsWith("." + trimmedDomain, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        private bool IsPathMatch(Uri requestUri)
        {
            var cookiePath = GetCookiePath();
            if (cookiePath == "/")
            {
                return true;
            }

            var requestPath = (requestUri.AbsolutePath.Length > 0) ? requestUri.AbsolutePath : "/";

            //case-sensitive
            if (requestPath.Equals(cookiePath, StringComparison.Ordinal))
            {
                return true;
            }
            if (requestPath.Length > cookiePath.Length && requestPath.StartsWith(cookiePath, StringComparison.Ordinal) && requestPath[cookiePath.Length] == '/')
            {
                return true;
            }

            return false;
        }

        private string GetCookiePath()
        {
            string cookiePath = _path;
            if (string.IsNullOrEmpty(cookiePath) || !cookiePath.StartsWith("/", StringComparison.Ordinal))
            {
                return cookiePath = GetDefaultPath();
            }
            if (cookiePath != null && cookiePath.EndsWith("/", StringComparison.Ordinal))
            {
                cookiePath = cookiePath.TrimEnd('/');
            }
            return cookiePath;
        }

        private string GetDefaultPath()
        {
            string cookiePath = _originUri.AbsolutePath;
            if (string.IsNullOrEmpty(cookiePath))
            {
                return "/";
            }
            int lastIndex = cookiePath.LastIndexOf('/', 1);
            if (cookiePath[0] != '/' || lastIndex < 0)
            {
                return "/";
            }
            return cookiePath.Substring(0, lastIndex);
        }

        public override string ToString()
        {
            return $"{_name}={_value}";
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
            var properties = HeaderProperty.ParseList(headerValue);
            if (properties == null || properties.Length == 0)
            {
                cookie = null;
                return false;
            }
            cookie = new Cookie(properties[0].Name, properties[0].Value, originUri, receievedDate);

            properties = properties.Skip(1).ToArray();
            cookie.ParseAdditionalProperties(properties);
            return cookie.GetValidity() == CookieValidity.Valid;
        }

        private void ParseAdditionalProperties(HeaderProperty[] properties)
        {
            if (properties.TryGet("Domain", out string domain))
            {
                _domain = domain;
            }
            if (properties.TryGet("Path", out string path))
            {
                _path = path;
            }

            if (properties.TryGet("HttpOnly", out _))
            {
                _httpOnly = true;
            }
            if (properties.TryGet("Secure", out _))
            {
                _secure = true;
            }
            if (properties.TryGet("SameSite", out string sameSiteString) && Enum.TryParse(sameSiteString, true, out SameSite sameSite))
            {
                _samesite = sameSite;
            }

            if (properties.TryGet("Expires", out string expiresString) && DateTimeOffset.TryParse(expiresString, out DateTimeOffset expires))
            {
                _expires = expires;
            }
            if (properties.TryGet("Max-Age", out string maxAgeString) && int.TryParse(maxAgeString, out int maxAge))
            {
                _maxAge = maxAge;
            }
        }
    }
}
