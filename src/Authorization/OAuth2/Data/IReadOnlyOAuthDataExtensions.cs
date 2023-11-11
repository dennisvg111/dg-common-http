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
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsCompleted(this IReadOnlyOAuthData request)
        {
            return !string.IsNullOrEmpty(request.AccessToken);
        }

        /// <summary>
        /// Returns a value indicating if this authorization is still valid, based on <see cref="IReadOnlyOAuthData.ValidUntill"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsValid(this IReadOnlyOAuthData request)
        {
            return request.ValidUntill >= DateTimeOffset.UtcNow;
        }
    }
}
