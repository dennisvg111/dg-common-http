using System;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// Defines the properties a cookie needs to contain in order to be used in a <see cref="CookieJar"/>
    /// </summary>
    public interface ICookieIngredients
    {
        /// <summary>
        /// The name of this cookie.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The value of this cookie.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// The <see cref="Uri"/> from where this cookie was originally set. This must be an absolute URI.
        /// </summary>
        Uri OriginUri { get; }

        /// <summary>
        /// The date when this cookie was originally set.
        /// </summary>
        DateTimeOffset ReceivedDate { get; }

        /// <summary>
        /// Defines the host to which the cookie will be sent.
        /// </summary>
        string Domain { get; }

        /// <summary>
        /// Indicates the path that must exist in the requested URI for the cookie to be sent in the <c>Cookie</c> header.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Indicates that the cookie is sent to the server only when a request is made with the https: scheme (except on localhost), and therefore, is more resistant to man-in-the-middle attacks.
        /// </summary>
        bool IsSecure { get; }

        /// <summary>
        /// Forbids in-browser scripts from accessing the cookie.
        /// </summary>
        bool HttpOnly { get; }

        /// <summary>
        /// Controls whether or not a cookie is sent with cross-site requests, providing some protection against cross-site request forgery attacks (CSRF).
        /// </summary>
        SameSitePolicy SameSitePolicy { get; }

        /// <summary>
        /// <para>Indicates the maximum lifetime of the cookie as a timestamp.</para>
        /// <para>If both <see cref="Expires"/> and <see cref="MaxAge"/> are set, <see cref="MaxAge"/> has precedence.</para>
        /// </summary>
        DateTimeOffset? Expires { get; }

        /// <summary>
        /// <para>Indicates the number of seconds until the cookie expires. A zero or negative number will expire the cookie immediately.</para>
        /// <para>If both <see cref="Expires"/> and <see cref="MaxAge"/> are set, <see cref="MaxAge"/> has precedence.</para>
        /// </summary>
        int? MaxAge { get; }
    }
}
