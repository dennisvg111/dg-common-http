using DG.Common.Http.Authorization.OAuth2.Exceptions;
using System;
using System.Threading.Tasks;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// Represents the OAuth2 authorization flow for a single user.
    /// </summary>
    public class OAuthFlow
    {
        private readonly OAuthFlowHandler _handler;

        private readonly string[] _scopes;
        private readonly Uri _redirectUri;

        private Uri _authorizationUri;
        private string _state;

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthFlow"/> with the given <see cref="OAuthFlowHandler"/>, <paramref name="scopes"/>, and <paramref name="redirectUri"/>.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="scopes"></param>
        /// <param name="redirectUri"></param>
        public OAuthFlow(OAuthFlowHandler handler, string[] scopes, Uri redirectUri)
        {
            _handler = handler;
            _scopes = scopes;
            _redirectUri = redirectUri;

            _authorizationUri = _handler.GetAuthorizationUri(scopes, redirectUri, out _state);
        }

        /// <summary>
        /// Restarts this authorization flow.
        /// </summary>
        /// <param name="newAuthorizationUri"></param>
        public void Restart(out Uri newAuthorizationUri)
        {
            _authorizationUri = _handler.GetAuthorizationUri(_scopes, _redirectUri, out _state);
            newAuthorizationUri = _authorizationUri;
        }

        /// <summary>
        /// Returns the authorization url created for this <see cref="OAuthFlow"/>.
        /// </summary>
        /// <returns></returns>
        public Uri GetAuthorizationUri()
        {
            return _authorizationUri;
        }

        /// <summary>
        /// Processes the given <paramref name="code"/>, and saves the resulting <see cref="OAuthToken"/> to the current repository.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task AuthenticationCallback(string code)
        {
            await _handler.AuthenticationCallback(_state, code);
        }

        /// <summary>
        /// Returns a value indicating if authorization is granted for this <see cref="OAuthFlow"/>.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsAuthenticated()
        {
            return await _handler.IsAuthenticated(_state);
        }

        /// <summary>
        /// Returns a new <see cref="AuthorizationHeaderValue"/> for this <see cref="OAuthFlow"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="OAuthRequestNotFoundException"></exception>
        /// <exception cref="OAuthRequestNotCompletedException"></exception>
        /// <exception cref="OAuthAuthorizationExpiredException"></exception>
        public async Task<AuthorizationHeaderValue> GetAuthorizationHeaderAsync()
        {
            return await _handler.GetAuthorizationHeaderAsync(_state);
        }
    }
}
