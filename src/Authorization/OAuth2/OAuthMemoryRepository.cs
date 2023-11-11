using System.Collections.Concurrent;
using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Http.Authorization.OAuth2.Interfaces;

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

        private readonly ConcurrentDictionary<string, OAuthData> _requests = new ConcurrentDictionary<string, OAuthData>();

        /// <inheritdoc/>
        public void SaveRequest(OAuthData request)
        {
            _requests[request.State] = request;
        }

        /// <inheritdoc/>
        public bool TryGetRequestByState(string state, out OAuthData request)
        {
            return _requests.TryGetValue(state, out request);
        }
    }
}
