using DG.Common.Http.Headers;
using System;
using System.Globalization;
using System.Linq;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// This class represents the read-only information from a Set-Cookie header, to be used in a <see cref="CookieWrapper"/>.
    /// </summary>
    internal class ReadOnlyCookie : ICookie
    {
        private readonly string _name;

        private readonly string _value;
        private readonly Uri _originUri;

        private string _domain;
        private string _path;

        private readonly DateTimeOffset _receivedDate;
        private DateTimeOffset? _expires;
        private int? _maxAge; //in seconds

        private bool _secure;
        private bool _httpOnly;
        private SameSitePolicy _policy = SameSitePolicy.Lax;

        /// <inheritdoc/>
        public string Name => _name;

        /// <inheritdoc/>
        public string Value => _value;

        /// <inheritdoc/>
        public Uri OriginUri => _originUri;

        /// <inheritdoc/>
        public string Path => _path;

        /// <inheritdoc/>
        public string Domain => _domain;

        /// <inheritdoc/>
        public DateTimeOffset ReceivedDate => _receivedDate;

        /// <inheritdoc/>
        public bool IsSecure => _secure;

        /// <inheritdoc/>
        public bool HttpOnly => _httpOnly;

        /// <inheritdoc/>
        public SameSitePolicy SameSitePolicy => _policy;

        /// <inheritdoc/>
        public DateTimeOffset? Expires => _expires;

        /// <inheritdoc/>
        public int? MaxAge => _maxAge;

        private ReadOnlyCookie(string name, string value, Uri originUri, DateTimeOffset receivedDate)
        {
            _name = name;
            _value = value;
            _originUri = originUri;
            _receivedDate = receivedDate;
        }

        /// <summary>
        /// Parses a HTTP Set-Cookie header value to an instance of <see cref="CookieWrapper"/>, and returns a value indicating if parsing succeeded.
        /// </summary>
        /// <param name="headerValue"></param>
        /// <param name="receievedDate"></param>
        /// <param name="originUri"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool TryParse(string headerValue, DateTimeOffset receievedDate, Uri originUri, out ReadOnlyCookie cookie)
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
            cookie = new ReadOnlyCookie(properties[0].Name, properties[0].Value, originUri, receievedDate);

            properties = properties.Skip(1).ToArray();
            cookie.ParseAdditionalProperties(properties);
            return true;
        }

        private void ParseAdditionalProperties(HeaderValuePart[] properties)
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
            if (properties.TryGet("SameSite", out string sameSiteString) && Enum.TryParse(sameSiteString, true, out SameSitePolicy policy))
            {
                _policy = policy;
            }

            if (properties.TryGet("Expires", out string expiresString) && DateTimeOffset.TryParse(expiresString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset expires))
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
