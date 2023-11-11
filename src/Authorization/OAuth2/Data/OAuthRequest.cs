﻿using System;

namespace DG.Common.Http.Authorization.OAuth2.Data
{
    /// <summary>
    /// A request to start an authorization process.
    /// </summary>
    public class OAuthRequest
    {
        /// <summary>
        /// <inheritdoc cref="IReadOnlyOAuthData.State"/>
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// <inheritdoc cref="IReadOnlyOAuthData.RedirectUri"/>
        /// </summary>
        public Uri RedirectUri { get; set; }

        /// <summary>
        /// <inheritdoc cref="IReadOnlyOAuthData.Scopes"/>
        /// </summary>
        public string[] Scopes { get; set; }

        private static string GenerateState()
        {
            return Uulsid.NewUulsid().ToString();
        }

        internal static OAuthRequest NewFor(string[] scope, Uri redirectUri)
        {
            return new OAuthRequest()
            {
                State = GenerateState(),
                Scopes = scope,
                RedirectUri = redirectUri
            };
        }
    }
}
