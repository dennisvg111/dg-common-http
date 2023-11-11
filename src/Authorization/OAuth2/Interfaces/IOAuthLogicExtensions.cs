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
        /// <returns></returns>
        public static OAuthFlow StartNewFlow(this IOAuthLogic oauthLogic, string[] scopes, Uri callBackUri)
        {
            var request = OAuthRequest.NewFor(scopes, callBackUri);

            var data = OAuthData.ForNewRequest(request);

            return new OAuthFlow(oauthLogic, data);
        }
    }
}
