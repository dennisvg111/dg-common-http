namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// Controls whether or not a cookie is sent with cross-site requests, providing some protection against cross-site request forgery attacks (CSRF).
    /// </summary>
    public enum SameSitePolicy
    {
        /// <summary>
        /// <para>Means that the cookie is not sent on cross-site requests, such as on requests to load images or frames, but is sent when a user is navigating to the origin site from an external site (for example, when following a link).</para>
        /// <para>This is the default behavior if the SameSite attribute is not specified.</para>
        /// </summary>
        Lax,
        /// <summary>
        /// <para>Means that the browser sends the cookie only for same-site requests, that is, requests originating from the same site that set the cookie.</para>
        /// <para>If a request originates from a different domain or scheme (even with the same domain), no cookies with the SameSite=Strict attribute are sent.</para>
        /// </summary>
        Strict,
        /// <summary>
        /// <para>means that the browser sends the cookie with both cross-site and same-site requests.</para>
        /// <para>The Secure attribute must also be set when setting this value, like so 'SameSite=None; Secure'.</para>
        /// </summary>
        None
    }
}
