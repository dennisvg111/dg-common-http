using DG.Common.Exceptions;
using DG.Common.Http.Extensions;
using System;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// Provides extra functionality for an <see cref="ICookie"/>.
    /// </summary>
    public class CookieWrapper
    {
        private readonly ICookie _base;
        private readonly CookieUriData _path;

        /// <summary>
        /// The internal <see cref="ICookie"/> in this wrapper.
        /// </summary>
        public ICookie Cookie => _base;

        /// <summary>
        /// Initializes a new instance of <see cref="CookieWrapper"/> using the given cookie properties.
        /// </summary>
        /// <param name="cookieBase"></param>
        public CookieWrapper(ICookie cookieBase)
        {
            ThrowIf.Parameter.IsNull(cookieBase, nameof(cookieBase));
            ThrowIf.Parameter.IsNull(cookieBase.OriginUri, nameof(cookieBase.OriginUri));
            ThrowIf.Parameter.Matches(cookieBase.OriginUri, (uri) => !uri.IsAbsoluteUri, nameof(cookieBase.OriginUri), "Parameter must be an absolute URI.");
            ThrowIf.Parameter.IsNullOrEmpty(cookieBase.Name, nameof(cookieBase.Name));

            _base = new ReadOnlyCookie(cookieBase);
            _path = new CookieUriData(cookieBase);
        }

        /// <summary>
        /// Returns a value indicating if this cookies applies to the given <paramref name="requestUri"/>.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public bool AppliesTo(Uri requestUri)
        {
            if (_base.IsSecure && !requestUri.IsSecure())
            {
                return false;
            }
            return !_base.IsExpired() && _path.IsMatch(requestUri);
        }
    }
}
