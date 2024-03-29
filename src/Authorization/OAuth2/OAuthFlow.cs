﻿using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Http.Authorization.OAuth2.Exceptions;
using DG.Common.Http.Authorization.OAuth2.Interfaces;
using DG.Common.Http.Fluent;
using DG.Common.Threading;
using System;
using System.Threading.Tasks;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// <para>Represents the OAuth2 flow for a single authorization request.</para>
    /// <para>Note that this class also implements <see cref="IAuthorizationHeaderProvider"/>.</para>
    /// </summary>
    public class OAuthFlow<T> : IAuthorizationHeaderProvider where T : IOAuthLogic
    {
        /// <summary>
        /// An event that gets invoked when this <see cref="OAuthFlow{T}"/> updates the access token.
        /// </summary>
        public event EventHandler<UpdateEventArgs> OnUpdate;

        private readonly T _logic;

        private IReadOnlyOAuthData _data;

        /// <summary>
        /// The state value of this authorization flow.
        /// </summary>
        public string State => _data.State;

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthFlow{T}"/> with the given <paramref name="data"/>.
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="data"></param>
        public OAuthFlow(T logic, IReadOnlyOAuthData data)
        {
            _logic = logic;
            _data = data;
        }

        /// <summary>
        /// Returns the authorization url created for this <see cref="OAuthFlow{T}"/>.
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
        /// Returns a value indicating if authorization is granted for this <see cref="OAuthFlow{T}"/>.
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
        /// Returns a new <see cref="FluentAuthorization"/> for this <see cref="OAuthFlow{T}"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="OAuthRequestNotCompletedException"></exception>
        /// <exception cref="OAuthAuthorizationExpiredException"></exception>
        public async Task<FluentAuthorization> GetAuthorizationHeaderAsync()
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

    /// <summary>
    /// Provides methods to create new instances of <see cref="OAuthFlow{T}"/>.
    /// </summary>
    public static class OAuthFlow
    {
        /// <summary>
        /// Starts a new authorization flow using <typeparamref name="T"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static OAuthFlow<T> StartNew<T>(OAuthRequest request) where T : IOAuthLogic, new()
        {
            var data = OAuthData.ForNewRequest(request);
            var logic = new T();

            return new OAuthFlow<T>(logic, data);
        }

        /// <summary>
        /// Starts a new authorization flow using <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scopes"></param>
        /// <param name="callBackUri"></param>
        /// <returns></returns>
        public static OAuthFlow<T> StartNew<T>(string[] scopes, Uri callBackUri) where T : IOAuthLogic, new()
        {
            var request = new OAuthRequest()
            {
                State = OAuthState.NewState(),
                Scopes = scopes,
                CallBackUri = callBackUri
            };
            return StartNew<T>(request);
        }

        /// <summary>
        /// Returns a continuation of an authorization flow, using <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static OAuthFlow<T> Continue<T>(IReadOnlyOAuthData data) where T : IOAuthLogic, new()
        {
            var logic = new T();
            return new OAuthFlow<T>(logic, data);
        }

        /// <summary>
        /// Starts a new authorization flow using the given instance of <paramref name="logic"/>.
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static OAuthFlow<T> StartNewFor<T>(T logic, OAuthRequest request) where T : IOAuthLogic
        {
            var data = OAuthData.ForNewRequest(request);

            return new OAuthFlow<T>(logic, data);
        }

        /// <summary>
        /// Starts a new authorization flow using the given instance of <paramref name="logic"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logic"></param>
        /// <param name="scopes"></param>
        /// <param name="callBackUri"></param>
        /// <returns></returns>
        public static OAuthFlow<T> StartNewFor<T>(T logic, string[] scopes, Uri callBackUri) where T : IOAuthLogic
        {
            var request = new OAuthRequest()
            {
                State = OAuthState.NewState(),
                Scopes = scopes,
                CallBackUri = callBackUri
            };
            return StartNewFor(logic, request);
        }

        /// <summary>
        /// Returns a continuation of an authorization flow, using the given instance of <paramref name="logic"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logic"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static OAuthFlow<T> ContinueFor<T>(T logic, IReadOnlyOAuthData data) where T : IOAuthLogic
        {
            return new OAuthFlow<T>(logic, data);
        }
    }
}
