namespace DG.Common.Http.Cookies
{
    public enum CookieValidity
    {
        /// <summary>
        /// This is a valid cookie.
        /// </summary>
        Valid,
        /// <summary>
        /// If the cookie is marked as secure the origin uri must also be secure.
        /// </summary>
        OriginUriMustBeSecure,

    }
}
