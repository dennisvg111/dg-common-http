using System;

namespace DG.Common.Http.Authorization.OAuth2.Data
{
    /// <summary>
    /// Provides extension methods for <see cref="IReadOnlyOAuthData"/>
    /// </summary>
    public static class IReadOnlyOAuthDataExtensions
    {
        /// <summary>
        /// Returns a value indicating if this authorization has been completed, and <see cref="IReadOnlyOAuthData.AccessToken"/> can be used.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsCompleted(this IReadOnlyOAuthData data)
        {
            return !string.IsNullOrEmpty(data.AccessToken);
        }

        /// <summary>
        /// Returns a value indicating if this authorization has a refresh token.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool HasRefreshToken(this IReadOnlyOAuthData data)
        {
            return !string.IsNullOrEmpty(data.RefreshToken);
        }

        /// <summary>
        /// Returns a value indicating if this authorization is still valid, based on <see cref="IReadOnlyOAuthData.ValidUntill"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsValid(this IReadOnlyOAuthData data)
        {
            if (!data.ValidUntill.HasValue)
            {
                return true;
            }
            return data.ValidUntill.Value >= DateTimeOffset.UtcNow;
        }
    }
}
