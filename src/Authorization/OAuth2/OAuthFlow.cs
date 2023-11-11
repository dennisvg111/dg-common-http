using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Http.Authorization.OAuth2.Exceptions;
using DG.Common.Http.Authorization.OAuth2.Interfaces;
using System;
using System.Threading.Tasks;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// Represents the OAuth2 flow for a single authorization request.
    /// </summary>
    public class OAuthFlow
    {
        /// <summary>
        /// An event that gets invoked when this <see cref="OAuthFlow"/> refreshes the token.
        /// </summary>
        public event EventHandler<RefreshEventArgs> OnRefresh;

        private readonly IOAuthLogic _logic;

        private IReadOnlyOAuthData _request;
        private bool _updated = false;

        /// <summary>
        /// The state value of this authorization flow.
        /// </summary>
        public string State => _request.State;

        /// <summary>
        /// Indicates if the internal access token has been updated since this instance of <see cref="OAuthFlow"/> was initialized.
        /// </summary>
        internal bool IsUpdated => _updated;

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthFlow"/> with the given <see cref="IOAuthLogic"/>, and for the given request.
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="request"></param>
        public OAuthFlow(IOAuthLogic logic, IReadOnlyOAuthData request)
        {
            _logic = logic;
            _request = request;
        }

        /// <summary>
        /// Returns the authorization url created for this <see cref="OAuthFlow"/>.
        /// </summary>
        /// <returns></returns>
        public Uri GetAuthorizationUri()
        {
            return _logic.BuildAuthenticationUrlFor(OAuthData.From(_request));
        }

        /// <summary>
        /// Processes the given <paramref name="code"/>, and saves the resulting <see cref="OAuthToken"/> to the current repository.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task AuthenticationCallback(string code)
        {
            var token = await _logic.GetAccessTokenAsync(OAuthData.From(_request), code).ConfigureAwait(false);
            UpdateRequestWith(token);
        }

        /// <summary>
        /// Returns a value indicating if authorization is granted for this <see cref="OAuthFlow"/>.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsAuthenticated()
        {

            if (!_request.IsCompleted())
            {
                return false;
            }

            if (_request.IsValid())
            {
                return true;
            }

            var canRefresh = await RefreshAccessTokenAsync().ConfigureAwait(false);
            return canRefresh && _request.IsCompleted() && _request.IsValid();
        }

        /// <summary>
        /// Returns a new <see cref="AuthorizationHeaderValue"/> for this <see cref="OAuthFlow"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="OAuthRequestNotCompletedException"></exception>
        /// <exception cref="OAuthAuthorizationExpiredException"></exception>
        public async Task<AuthorizationHeaderValue> GetAuthorizationHeaderAsync()
        {

            if (!_request.IsCompleted())
            {
                OAuthRequestNotCompletedException.ThrowForState(_request.State);
            }

            if (!_request.IsValid())
            {
                var canRefresh = await RefreshAccessTokenAsync().ConfigureAwait(false);
                if (!canRefresh)
                {
                    OAuthAuthorizationExpiredException.ThrowForState(_request.State);
                }
            }

            return _logic.GetHeaderForToken(_request.AccessToken);
        }

        /// <summary>
        /// Refreshes the access token of this authorization flow.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RefreshAccessTokenAsync()
        {
            var canRefresh = await _logic.RefreshTokenAsync(_request.RefreshToken, out OAuthToken token).ConfigureAwait(false);
            if (!canRefresh)
            {
                return false;
            }
            UpdateRequestWith(token);
            return true;
        }

        private void UpdateRequestWith(OAuthToken token)
        {
            var newRequest = OAuthData.From(_request);
            newRequest.UpdateWith(token);
            _request = newRequest;
            _updated = true;
            OnRefresh?.Invoke(this, RefreshEventArgs.For(_request));
        }

        /// <summary>
        /// Returns an instance of <see cref="OAuthData"/> representing the current state of this authorization flow.
        /// </summary>
        /// <returns></returns>
        public OAuthData ExportRequest()
        {
            return OAuthData.From(_request);
        }
    }
}
