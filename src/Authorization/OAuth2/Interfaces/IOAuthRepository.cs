using DG.Common.Http.Authorization.OAuth2.Data;

namespace DG.Common.Http.Authorization.OAuth2.Interfaces
{
    /// <summary>
    /// Defines a way save and retreive <see cref="OAuthData"/> instances.
    /// </summary>
    public interface IOAuthRepository
    {
        /// <summary>
        /// Creates or updates the given <paramref name="request"/>.
        /// </summary>
        /// <param name="request"></param>
        void SaveRequest(OAuthData request);

        /// <summary>
        /// Tries to retrieve the <paramref name="request"/> with the given <paramref name="state"/> from the repository, and returns a value indicating if this was successful.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        bool TryGetRequestByState(string state, out OAuthData request);
    }
}
