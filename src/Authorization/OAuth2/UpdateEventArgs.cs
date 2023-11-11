using DG.Common.Http.Authorization.OAuth2.Data;
using System;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// Represents event data for when an OAuth token is updated.
    /// </summary>
    public class UpdateEventArgs : EventArgs
    {
        private readonly IReadOnlyOAuthData _data;

        /// <summary>
        /// The authorization data, with updated values for <see cref="IReadOnlyOAuthData.AccessToken"/>, <see cref="IReadOnlyOAuthData.RefreshToken"/>, and <see cref="IReadOnlyOAuthData.ValidUntill"/>.
        /// </summary>
        public IReadOnlyOAuthData Data => _data;

        /// <summary>
        /// The state value of the authorization that was updated.
        /// </summary>
        public string State => _data.State;

        internal UpdateEventArgs(IReadOnlyOAuthData request)
        {
            _data = request;
        }

        internal static UpdateEventArgs For(IReadOnlyOAuthData request)
        {
            return new UpdateEventArgs(request);
        }
    }
}
