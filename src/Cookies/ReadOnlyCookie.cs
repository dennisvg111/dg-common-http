using System;

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

        private readonly string _domain;
        private readonly string _path;

        private readonly DateTimeOffset _receivedDate;
        private readonly DateTimeOffset? _expires;
        private readonly int? _maxAge; //in seconds

        private readonly bool _secure;
        private readonly bool _httpOnly;
        private readonly SameSitePolicy _policy = SameSitePolicy.Lax;

        /// <inheritdoc/>
        public string Name => _name;

        /// <inheritdoc/>
        public string Value => _value;

        /// <inheritdoc/>
        public Uri OriginUri => _originUri;

        /// <inheritdoc/>
        public string Domain => _domain;

        /// <inheritdoc/>
        public string Path => _path;

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

        internal ReadOnlyCookie(ICookie cookie)
            : this(cookie.Name, cookie.Value, cookie.OriginUri == null ? null : new UriBuilder(cookie.OriginUri).Uri, cookie.ReceivedDate)
        {
            _domain = cookie.Domain;
            _path = cookie.Path;
            _expires = cookie.Expires;
            _maxAge = cookie.MaxAge;
            _secure = cookie.IsSecure;
            _httpOnly = cookie.HttpOnly;
            _policy = cookie.SameSitePolicy;
        }
    }
}
