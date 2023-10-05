using DG.Common.Exceptions;
using System;

namespace DG.Common.Http.Cookies
{
    public class CookieIngredients : ICookieIngredients
    {
        private readonly string _name;
        private readonly string _value;
        private readonly Uri _originUri;
        private readonly DateTimeOffset _receivedDate;

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
        public SameSitePolicy SameSitePolicy { get; set; } = SameSitePolicy.Lax;

        /// <inheritdoc/>
        public DateTimeOffset? Expires { get; set; }

        /// <inheritdoc/>
        public int? MaxAge { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="CookieIngredients"/> with the given name, value, and creation data.
        /// </summary>
        /// <param name="name">The name of this cookie.</param>
        /// <param name="value">The value of this cookie.</param>
        /// <param name="originUri">The URI of the response for which this cookie was set.</param>
        /// <param name="receivedDate">The date this cookie was originally received.</param>
        public CookieIngredients(string name, string value, Uri originUri, DateTimeOffset receivedDate)
        {
            ThrowIf.Parameter.IsNullOrEmpty(name, nameof(name));
            ThrowIf.Parameter.IsNullOrEmpty(value, nameof(value));
            ThrowIf.Parameter.IsNull(originUri, nameof(originUri));
            ThrowIf.Parameter.Matches(originUri, (uri) => !uri.IsAbsoluteUri, nameof(originUri), "Parameter must be an absolute URI.");

            _name = name;
            _value = value;
            _originUri = originUri;
            _receivedDate = receivedDate;
        }

        /// <summary>
        /// Creates a copy of this instance of <see cref="CookieIngredients"/>.
        /// </summary>
        /// <returns></returns>
        public CookieIngredients Copy()
        {
            return new CookieIngredients(Name, Value, OriginUri, ReceivedDate)
            {
                Domain = Domain,
                Path = Path,
                IsSecure = IsSecure,
                HttpOnly = HttpOnly,
                SameSitePolicy = SameSitePolicy,
                Expires = Expires,
                MaxAge = MaxAge
            };
        }

        public static CookieIngredients Copy(ICookieIngredients cookie)
        {
            return new CookieIngredients(cookie.Name, cookie.Value, cookie.OriginUri, cookie.ReceivedDate)
            {
                Domain = cookie.Domain,
                Path = cookie.Path,
                IsSecure = cookie.IsSecure,
                HttpOnly = cookie.HttpOnly,
                SameSitePolicy = cookie.SameSitePolicy,
                Expires = cookie.Expires,
                MaxAge = cookie.MaxAge
            };
        }

        /// <summary>
        /// Returns a read-only finished <see cref="Cookie"/> instance.
        /// </summary>
        /// <returns></returns>
        public Cookie Bake()
        {
            return new Cookie(Copy());
        }
    }
}
