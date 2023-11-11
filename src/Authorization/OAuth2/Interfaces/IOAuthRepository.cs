using DG.Common.Http.Authorization.OAuth2.Data;

namespace DG.Common.Http.Authorization.OAuth2.Interfaces
{
    /// <summary>
    /// Defines a way save and retreive <see cref="OAuthData"/> instances.
    /// </summary>
    public interface IOAuthRepository
    {
        /// <summary>
        /// Creates or updates the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data"></param>
        void Save(OAuthData data);

        /// <summary>
        /// Tries to retrieve the <paramref name="data"/> with the given <paramref name="state"/> from the repository, and returns a value indicating if this was successful.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool TryGetByState(string state, out OAuthData data);
    }
}
