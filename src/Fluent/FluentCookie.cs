using DG.Common.Http.Cookies;
using DG.Common.Http.Extensions;
using System;
using System.Text;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// An implementation of <see cref="ICookie"/> with methods that allow for chaining calls.
    /// </summary>
    public class FluentCookie : ICookie
    {
        private readonly string _name;
        private readonly string _value;

        private readonly Uri _originUri;
        private readonly DateTimeOffset _receivedDate;

        private SameSitePolicy? _sameSitePolicy;

        /// <inheritdoc/>
        public string Name => _name;

        /// <inheritdoc/>
        public string Value => _value;

        /// <inheritdoc/>
        public Uri OriginUri => _originUri;

        /// <inheritdoc/>
        public DateTimeOffset ReceivedDate => _receivedDate;

        /// <inheritdoc/>
        public string Domain { get; set; }

        /// <inheritdoc/>
        public string Path { get; set; }

        /// <inheritdoc/>
        public bool IsSecure { get; set; }

        /// <inheritdoc/>
        public bool HttpOnly { get; set; }

        /// <inheritdoc/>
        public SameSitePolicy SameSitePolicy
        {
            get
            {
                return _sameSitePolicy ?? SameSitePolicy.Lax;
            }
            set
            {
                _sameSitePolicy = value;
            }
        }

        /// <inheritdoc/>
        public DateTimeOffset? Expires { get; set; }

        /// <inheritdoc/>
        public int? MaxAge { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentCookie"/> with the specified name, value, origin URI, and <see cref="DateTimeOffset.UtcNow"/> as the received date.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="originUri">The URI of the origin associated with the cookie. Can be null if no origin is specified.</param>
        public FluentCookie(string name, string value, Uri originUri)
            : this(name, value, originUri, DateTimeOffset.UtcNow) { }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentCookie"/> with the specified name, value, origin URI, and received date.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="originUri">The URI of the origin associated with the cookie.</param>
        /// <param name="receivedDate">The date and time when the cookie was received.</param>
        public FluentCookie(string name, string value, Uri originUri, DateTimeOffset receivedDate)
        {
            _name = name;
            _value = value;
            _originUri = originUri
            _receivedDate = receivedDate;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentCookie"/> by copying the properties of the specified <see cref="ICookie"/>.
        /// </summary>
        /// <param name="cookie">The <see cref="ICookie"/> instance whose properties are used to initialize this <see cref="FluentCookie"/>.</param>
        public FluentCookie(ICookie cookie)
            : this(cookie.Name, cookie.Value, cookie.OriginUri, cookie.ReceivedDate)
        {
            Domain = cookie.Domain;
            Path = cookie.Path;
            Expires = cookie.Expires;
            MaxAge = cookie.MaxAge;
            IsSecure = cookie.IsSecure;
            HttpOnly = cookie.HttpOnly;
            SameSitePolicy = cookie.SameSitePolicy;
        }

        /// <summary>
        /// Returns a copy of this cookie with <see cref="Domain"/> set to <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public FluentCookie WithDomain(string domain)
        {
            return new FluentCookie(this)
            {
                Domain = domain
            };
        }

        /// <summary>
        /// Returns a copy of this cookie with <see cref="Path"/> set to <paramref name="path"/>.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public FluentCookie WithPath(string path)
        {
            return new FluentCookie(this)
            {
                Path = path
            };
        }

        /// <summary>
        /// Returns a copy of this cookie with <see cref="HttpOnly"/> set to <paramref name="httpOnly"/>.
        /// </summary>
        /// <returns></returns>
        public FluentCookie WithHttpOnlyValue(bool httpOnly)
        {
            return new FluentCookie(this)
            {
                HttpOnly = httpOnly
            };
        }

        /// <summary>
        /// Returns a copy of this cookie with <see cref="HttpOnly"/> set to <see langword="true"/>.
        /// </summary>
        /// <returns></returns>
        public FluentCookie WithHttpOnly()
        {
            return WithHttpOnlyValue(true);
        }

        /// <summary>
        /// Returns a copy of this cookie with <see cref="HttpOnly"/> set to <see langword="false"/>.
        /// </summary>
        /// <returns></returns>
        public FluentCookie WithoutHttpOnly()
        {
            return WithHttpOnlyValue(false);
        }

        /// <summary>
        /// Returns a copy of this cookie with <see cref="IsSecure"/> set to <paramref name="isSecure"/>.
        /// </summary>
        /// <returns></returns>
        public FluentCookie WithSecureValue(bool isSecure)
        {
            return new FluentCookie(this)
            {
                IsSecure = isSecure
            };
        }

        /// <summary>
        /// Returns a copy of this cookie with <see cref="IsSecure"/> set to <see langword="false"/>.
        /// </summary>
        /// <returns></returns>
        public FluentCookie WithSecure()
        {
            return WithSecureValue(true);
        }

        /// <summary>
        /// Returns a copy of this cookie with <see cref="IsSecure"/> set to <see langword="false"/>.
        /// </summary>
        /// <returns></returns>
        public FluentCookie WithoutSecure()
        {
            return WithSecureValue(false);
        }

        /// <summary>
        /// Returns a copy of this cookie with the <see cref="SameSitePolicy"/> set to <paramref name="policy"/>.
        /// </summary>
        /// <remarks>If <paramref name="policy"/> is <see cref="SameSitePolicy.None"/>, <see cref="IsSecure"/> will also be set to <see langword="true"/>.</remarks>
        /// <returns></returns>
        public FluentCookie WithSameSitePolicy(SameSitePolicy policy)
        {
            return new FluentCookie(this)
            {
                SameSitePolicy = policy,
                IsSecure = policy == SameSitePolicy.None ? true : IsSecure // If SameSite is None, Secure must be true
            };
        }

        /// <summary>
        /// Returns a copy of this cookie with the <see cref="Expires"/> set to <paramref name="expires"/>.
        /// </summary>
        /// <returns></returns>
        public FluentCookie WithExpiration(DateTimeOffset expires)
        {
            return new FluentCookie(this)
            {
                Expires = expires
            };
        }

        /// <summary>
        /// Returns a copy of this cookie with the <see cref="MaxAge"/> set to the given max age in seconds.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public FluentCookie WithMaxAge(int seconds)
        {
            return new FluentCookie(this)
            {
                MaxAge = seconds
            };
        }

        /// <summary>
        /// Returns a copy of this cookie with the <see cref="MaxAge"/> set to the given <paramref name="maxAge"/>.
        /// </summary>
        /// <param name="maxAge"></param>
        /// <returns></returns>
        public FluentCookie WithMaxAge(TimeSpan maxAge)
        {
            return new FluentCookie(this)
            {
                MaxAge = (int)maxAge.TotalSeconds
            };
        }

        /// <summary>
        /// Converts the cookie's properties into a valid <c>Set-Cookie</c> header value.
        /// </summary>
        /// <remarks>This header value only contains relevant properties of a cookie.</remarks>
        /// <returns></returns>
        public FluentHeader ConvertToSetCookieHeader()
        {
            // Construct the Set-Cookie header string based on the properties of the cookie
            StringBuilder setCookieBuilder = new StringBuilder($"{Name}={Value}");

            if (Domain != null)
            {
                setCookieBuilder.Append($"; Domain={Domain}");
            }

            if (Path != null)
            {
                setCookieBuilder.Append($"; Path={Path}");
            }

            if (Expires.HasValue)
            {
                setCookieBuilder.Append($"; Expires={Expires.Value.ToCookieExpiresString()}");
            }

            if (MaxAge.HasValue)
            {
                setCookieBuilder.Append($"; Max-Age={MaxAge.Value}");
            }

            if (IsSecure)
            {
                setCookieBuilder.Append("; Secure");
            }

            if (HttpOnly)
            {
                setCookieBuilder.Append("; HttpOnly");
            }

            if (_sameSitePolicy.HasValue)
            {
                setCookieBuilder.Append($"; SameSite={SameSitePolicy}");
            }

            return new FluentHeader("Set-Cookie", setCookieBuilder.ToString());
        }

        /// <summary>
        /// Returns a string that represents this cookie in the format used as a <c>Set-Cookie</c> header value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ConvertToSetCookieHeader().Value;
        }
    }
}
