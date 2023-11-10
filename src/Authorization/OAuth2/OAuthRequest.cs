using System;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// A request to start an authorization process.
    /// </summary>
    public class OAuthRequest
    {
        /// <summary>
        /// A random string generated to protect from CSRF attacks.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The <see cref="Uri"/> that the server should redirect to after the authorization prompt.
        /// </summary>
        public Uri RedirectUrl { get; set; }

        /// <summary>
        /// The scopes this client is asking for.
        /// </summary>
        public string[] Scopes { get; set; }
    }
}
