using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Threading;
using System;
using System.Threading.Tasks;

namespace DG.Common.Http.Authorization.OAuth2.Interfaces
{
    /// <summary>
    /// Defines functionality for managing an OAuth authorization request.
    /// </summary>
    public interface IOAuthLogic
    {
        /// <summary>
        /// Returns a new instance of <see cref="Uri"/> for the given <paramref name="request"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Uri BuildAuthenticationUrlFor(OAuthRequest request);

        /// <summary>
        /// Retrieves an <see cref="OAuthToken"/> based on the given <see cref="OAuthRequest"/>, and <paramref name="callBackCode"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callBackCode">The code returned to the callback url.</param>
        /// <returns></returns>
        Task<OAuthToken> GetAccessTokenAsync(OAuthRequest request, string callBackCode);

        /// <summary>
        /// Retrieves a refreshed <see cref="OAuthToken"/> based on the given <paramref name="refreshToken"/>..
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<TaskResult<OAuthToken>> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Returns a new <see cref="AuthorizationHeaderValue"/> for te given <paramref name="accessToken"/>.
        /// </summary>
        /// <param name="accessToken">The access_token (retreived from <see cref="OAuthToken.AccessToken"/>).</param>
        /// <returns></returns>
        AuthorizationHeaderValue GetHeaderForToken(string accessToken);
    }
}
