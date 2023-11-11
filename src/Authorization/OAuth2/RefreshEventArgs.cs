using DG.Common.Http.Authorization.OAuth2.Data;
using System;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// Represents event data for when an OAuth token is refreshed.
    /// </summary>
    public class RefreshEventArgs : EventArgs
    {
        private readonly IReadOnlyOAuthData _request;

        /// <summary>
        /// The request, with updated values for <see cref="IReadOnlyOAuthData.AccessToken"/>, <see cref="IReadOnlyOAuthData.RefreshToken"/>, and <see cref="IReadOnlyOAuthData.ValidUntill"/>.
        /// </summary>
        public IReadOnlyOAuthData Request => _request;

        /// <summary>
        /// The state value of the OAuth token that was refreshed.
        /// </summary>
        public string State => _request.State;

        internal RefreshEventArgs(IReadOnlyOAuthData request)
        {
            _request = request;
        }

        internal static RefreshEventArgs For(IReadOnlyOAuthData request)
        {
            return new RefreshEventArgs(request);
        }
    }
}
