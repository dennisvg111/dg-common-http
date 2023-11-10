namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// Defines a way save and retreive <see cref="SavedOAuthRequest"/> instances.
    /// </summary>
    public interface IOAuthRepository
    {
        /// <summary>
        /// Creates or updates the given <paramref name="request"/>.
        /// </summary>
        /// <param name="request"></param>
        void SaveRequest(SavedOAuthRequest request);

        /// <summary>
        /// Tries to retrieve the <paramref name="request"/> with the given <paramref name="state"/> from the repository, and returns a value indicating if this was successful.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        bool TryGetRequestByState(string state, out SavedOAuthRequest request);
    }
}
