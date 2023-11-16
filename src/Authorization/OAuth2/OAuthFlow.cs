using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Http.Authorization.OAuth2.Exceptions;
using DG.Common.Http.Authorization.OAuth2.Interfaces;
using DG.Common.Threading;
using System;
using System.Threading.Tasks;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// <para>Represents the OAuth2 flow for a single authorization request.</para>
    /// <para>Note that this class also implements <see cref="IAuthorizationHeaderProvider"/>.</para>
    /// </summary>
    public class OAuthFlow : IAuthorizationHeaderProvider
    {
        /// <summary>
        /// An event that gets invoked when this <see cref="OAuthFlow"/> updates the access token.
        /// </summary>
        public event EventHandler<UpdateEventArgs> OnUpdate;

        private readonly IOAuthLogic _logic;

        private IReadOnlyOAuthData _data;

        /// <summary>
        /// The state value of this authorization flow.
        /// </summary>
        public string State => _data.State;

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthFlow"/> for the given <see cref="IOAuthLogic"/>, and with the given <paramref name="data"/>.
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="data"></param>
        internal OAuthFlow(IOAuthLogic logic, IReadOnlyOAuthData data)
        {
            _logic = logic;
            _data = data;
        }

        /// <summary>
        /// Returns the authorization url created for this <see cref="OAuthFlow"/>.
        /// </summary>
        /// <returns></returns>
        public Uri GetAuthorizationUri()
        {
            return _logic.BuildAuthorizationUri(OAuthData.From(_data));
        }

        /// <summary>
        /// Processes the given <paramref name="code"/>, and saves the resulting <see cref="OAuthToken"/> to the current repository.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task AuthorizationCallbackAsync(string code)
        {
            var token = await _logic.GetAccessTokenAsync(OAuthData.From(_data), code).ConfigureAwait(false);
            UpdateRequestWith(token);
        }

        /// <summary>
        /// Returns a value indicating if authorization is granted for this <see cref="OAuthFlow"/>.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsAuthorizedAsync()
        {

            if (!_data.IsCompleted())
            {
                return false;
            }

            if (!_data.IsExpired())
            {
                return true;
            }

            var canRefresh = await TryRefreshAccessTokenAsync().ConfigureAwait(false);
            return canRefresh && _data.IsCompleted() && !_data.IsExpired();
        }

        /// <summary>
        /// Returns a new <see cref="AuthorizationHeaderValue"/> for this <see cref="OAuthFlow"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="OAuthRequestNotCompletedException"></exception>
        /// <exception cref="OAuthAuthorizationExpiredException"></exception>
        public async Task<AuthorizationHeaderValue> GetAuthorizationHeaderAsync()
        {

            if (!_data.IsCompleted())
            {
                OAuthRequestNotCompletedException.ThrowForState(_data.State);
            }

            if (_data.IsExpired())
            {
                var canRefresh = await TryRefreshAccessTokenAsync().ConfigureAwait(false);
                if (!canRefresh)
                {
                    OAuthAuthorizationExpiredException.ThrowForState(_data.State);
                }
            }

            return _logic.GetHeaderForToken(_data.AccessToken);
        }

        /// <summary>
        /// Refreshes the access token of this authorization flow, and returns a value indicating if this was successful.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryRefreshAccessTokenAsync()
        {
            if (!_data.HasRefreshToken())
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }
            var tokenResult = await _logic.TryRefreshTokenAsync(_data.RefreshToken).ConfigureAwait(false);
            if (tokenResult.TryGet(out OAuthToken token))
            {
                UpdateRequestWith(token);
                return true;
            }

            if (!tokenResult.FailedBecauseOfException)
            {
                Invalidate();
            }

            return false;
        }

        /// <summary>
        /// Invalidates the current authorization.
        /// </summary>
        public void Invalidate()
        {
            var newRequest = OAuthData.From(_data);
            newRequest.AccessTokenExpirationDate = new DateTimeOffset(0, TimeSpan.Zero);
            newRequest.RefreshToken = null;
            _data = newRequest;
            OnUpdate?.Invoke(this, UpdateEventArgs.For(_data));
        }

        private void UpdateRequestWith(OAuthToken token)
        {
            var newRequest = OAuthData.From(_data);
            newRequest.UpdateWith(token);
            _data = newRequest;
            OnUpdate?.Invoke(this, UpdateEventArgs.For(_data));
        }

        /// <summary>
        /// Returns a new instance of <see cref="OAuthData"/> representing the current data of this authorization flow.
        /// </summary>
        /// <returns></returns>
        public OAuthData Export()
        {
            return OAuthData.From(_data);
        }

        /// <inheritdoc/>
        bool IAuthorizationHeaderProvider.IsAuthorized => SafeSync.Run(() => IsAuthorizedAsync());

        /// <inheritdoc/>
        string IAuthorizationHeaderProvider.GetAuthorizationHeaderValue()
        {
            var header = SafeSync.Run(() => GetAuthorizationHeaderAsync());
            return header.ToString();
        }
    }
}
