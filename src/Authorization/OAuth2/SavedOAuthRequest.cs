using System;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// Represents an authorization request saved to a <see cref="IOAuthRepository"/>.
    /// </summary>
    public class SavedOAuthRequest : OAuthRequest
    {
        /// <summary>
        /// The date this authorization request was started.
        /// </summary>
        public DateTimeOffset Started { get; set; }

        /// <summary>
        /// The access token when this request finished.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// The date untill <see cref="AccessToken"/> is no longer valid.
        /// </summary>
        public DateTimeOffset ValidUntill { get; set; }

        /// <summary>
        /// The refresh token associated with this request.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Returns a value indicating if this request has been completed, and <see cref="AccessToken"/> can be used.
        /// </summary>
        internal bool IsCompleted => !string.IsNullOrEmpty(AccessToken);

        internal void UpdateWith(OAuthToken token)
        {
            AccessToken = token.AccessToken;
            ValidUntill = token.ValidUntill;
            if (!string.IsNullOrEmpty(token.RefreshToken))
            {
                RefreshToken = token.RefreshToken;
            }
        }
    }
}
