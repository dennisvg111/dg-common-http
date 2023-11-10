using System.Collections.Concurrent;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// An implementation of <see cref="IOAuthRepository"/> using an in-memory dictionary.
    /// </summary>
    public class OAuthMemoryRepository : IOAuthRepository
    {
        private static readonly OAuthMemoryRepository _defaultInstance = new OAuthMemoryRepository();

        /// <summary>
        /// Provides a shared instance of <see cref="OAuthMemoryRepository"/>.
        /// </summary>
        public static OAuthMemoryRepository Instance => _defaultInstance;

        private readonly ConcurrentDictionary<string, SavedOAuthRequest> _requests = new ConcurrentDictionary<string, SavedOAuthRequest>();

        /// <inheritdoc/>
        public void SaveRequest(SavedOAuthRequest request)
        {
            _requests[request.State] = request;
        }

        /// <inheritdoc/>
        public bool TryGetRequestByState(string state, out SavedOAuthRequest request)
        {
            return _requests.TryGetValue(state, out request);
        }
    }
}
