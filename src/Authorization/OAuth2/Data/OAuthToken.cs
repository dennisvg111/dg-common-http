using System;

namespace DG.Common.Http.Authorization.OAuth2.Data
{
    /// <summary>
    /// Represents the results of an OAuth2 authorization flow.
    /// </summary>
    public class OAuthToken
    {
        private readonly string _accessToken;
        private readonly DateTimeOffset? _validUntill;
        private readonly string _refreshToken;

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthToken"/> with the given <paramref name="accessToken"/> and optional <paramref name="refreshToken"/>, and <paramref name="validUntill"/> indicating untill when the access token is valid.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="validUntill"></param>
        /// <param name="refreshToken"></param>
        public OAuthToken(string accessToken, DateTimeOffset? validUntill, string refreshToken)
        {
            _accessToken = accessToken;
            _validUntill = validUntill;
            _refreshToken = refreshToken;
        }

        /// <summary>
        /// The access token string as issued by the authorization server.
        /// </summary>
        public string AccessToken => _accessToken;

        /// <summary>
        /// <para>The time untill which <see cref="AccessToken"/> is valid.</para>
        /// <para>If this property is <see langword="null"/>, it is assumed this <see cref="AccessToken"/> does not expire.</para>
        /// </summary>
        public DateTimeOffset? ValidUntill => _validUntill;

        /// <summary>
        /// An optional refresh token which applications can use to obtain another access token.
        /// </summary>
        public string RefreshToken => _refreshToken;

        /// <summary>
        /// Indicates if a <see cref="RefreshToken"/> is specified.
        /// </summary>
        public bool HasRefreshToken => !string.IsNullOrEmpty(_refreshToken);
    }
}
