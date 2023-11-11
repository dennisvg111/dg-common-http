﻿using DG.Common.Http.Authorization.OAuth2.Interfaces;
using System;

namespace DG.Common.Http.Authorization.OAuth2.Data
{
    /// <summary>
    /// Represents authorization data saved to a <see cref="IOAuthRepository"/>.
    /// </summary>
    public class OAuthData : OAuthRequest, IReadOnlyOAuthData
    {
        /// <summary>
        /// <inheritdoc cref="IReadOnlyOAuthData.Started"/>
        /// </summary>
        public DateTimeOffset Started { get; set; }

        /// <summary>
        /// <inheritdoc cref="IReadOnlyOAuthData.AccessToken"/>
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// <inheritdoc cref="IReadOnlyOAuthData.ValidUntill"/>
        /// </summary>
        public DateTimeOffset? ValidUntill { get; set; }

        /// <summary>
        /// <inheritdoc cref="IReadOnlyOAuthData.RefreshToken"/>
        /// </summary>
        public string RefreshToken { get; set; }

        internal void UpdateWith(OAuthToken token)
        {
            AccessToken = token.AccessToken;
            ValidUntill = token.ValidUntill;
            if (!string.IsNullOrEmpty(token.RefreshToken))
            {
                RefreshToken = token.RefreshToken;
            }
        }

        /// <summary>
        /// Returns a new instance of <see cref="OAuthData"/>, with the data set based on the given <paramref name="request"/> and <see cref="Started"/> set to <see cref="DateTimeOffset.Now"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static OAuthData ForNewRequest(OAuthRequest request)
        {
            var started = DateTimeOffset.UtcNow;
            return new OAuthData()
            {
                State = request.State,
                Scopes = request.Scopes,
                RedirectUri = request.RedirectUri,
                Started = started,
                AccessToken = null,
                RefreshToken = null,
                ValidUntill = null
            };
        }

        /// <summary>
        /// Returns a new instance of <see cref="OAuthData"/> with the same property values as <paramref name="request"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static OAuthData From(IReadOnlyOAuthData request)
        {
            return new OAuthData()
            {
                State = request.State,
                Scopes = request.Scopes,
                RedirectUri = request.RedirectUri,
                Started = request.Started,
                AccessToken = request.AccessToken,
                ValidUntill = request.ValidUntill,
                RefreshToken = request.RefreshToken
            };
        }
    }
}