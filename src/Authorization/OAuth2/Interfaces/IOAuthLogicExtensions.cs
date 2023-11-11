using DG.Common.Http.Authorization.OAuth2.Data;
using System;

namespace DG.Common.Http.Authorization.OAuth2.Interfaces
{
    /// <summary>
    /// Provides extension methods for <see cref="IOAuthLogic"/> implementations.
    /// </summary>
    public static class IOAuthLogicExtensions
    {
        /// <summary>
        /// Starts a new authorization flow with the current <see cref="IOAuthLogic"/>.
        /// </summary>
        /// <param name="oauthLogic"></param>
        /// <param name="scopes"></param>
        /// <param name="callBackUri"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static OAuthFlow StartAuthorization(this IOAuthLogic oauthLogic, string[] scopes, Uri callBackUri, out string state)
        {
            var request = OAuthRequest.NewFor(scopes, callBackUri);
            state = request.State;
            return new OAuthFlow(oauthLogic, OAuthData.From(request));
        }

        /// <summary>
        /// Starts a new authorization flow with the current <see cref="IOAuthLogic"/>.
        /// </summary>
        /// <param name="oauthLogic"></param>
        /// <param name="scopes"></param>
        /// <param name="callBackUri"></param>
        /// <returns></returns>
        public static OAuthFlow StartAuthorization(this IOAuthLogic oauthLogic, string[] scopes, Uri callBackUri)
        {
            return StartAuthorization(oauthLogic, scopes, callBackUri, out string _);
        }
    }
}
