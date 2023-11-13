using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Http.Authorization.OAuth2.Interfaces;
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

        private readonly ConcurrentDictionary<string, OAuthData> _requests = new ConcurrentDictionary<string, OAuthData>();

        /// <inheritdoc/>
        public void Save(OAuthData data)
        {
            _requests[data.State] = data;
        }

        /// <inheritdoc/>
        public bool TryGetByState(string state, out OAuthData data)
        {
            return _requests.TryGetValue(state, out data);
        }

        /// <inheritdoc/>
        public void RemoveDataByState(string state)
        {
            if (!_requests.ContainsKey(state))
            {
                return;
            }
            _requests.TryRemove(state, out OAuthData _);
        }
    }
}
