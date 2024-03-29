﻿using System;

namespace DG.Common.Http.Authorization.OAuth2.Data
{
    /// <summary>
    /// Defines a read-only version of OAuth2 authorization data.
    /// </summary>
    public interface IReadOnlyOAuthData
    {
        /// <summary>
        /// A random string generated to protect from CSRF attacks.
        /// </summary>
        string State { get; }

        /// <summary>
        /// The <see cref="Uri"/> that the authorization server should redirect to after the authorization prompt.
        /// </summary>
        Uri CallBackUri { get; }

        /// <summary>
        /// The scopes this client is asking for.
        /// </summary>
        string[] Scopes { get; }

        /// <summary>
        /// The date this authorization request was started.
        /// </summary>
        DateTimeOffset Started { get; }

        /// <summary>
        /// The access token when this request finished.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// <para>The moment <see cref="AccessToken"/> expires.</para>
        /// <para>If this property is <see langword="null"/>, it is assumed this <see cref="AccessToken"/> does not expire.</para>
        /// </summary>
        DateTimeOffset? AccessTokenExpirationDate { get; }

        /// <summary>
        /// The refresh token associated with this request.
        /// </summary>
        string RefreshToken { get; }
    }
}