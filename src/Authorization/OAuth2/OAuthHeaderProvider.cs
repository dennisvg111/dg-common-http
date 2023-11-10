using DG.Common.Threading;

namespace DG.Common.Http.Authorization.OAuth2
{
    public class OAuthHeaderProvider : IAuthorizationHeaderProvider
    {
        private readonly OAuthFlow _oAuthFlow;
        private readonly string _state;

        public OAuthHeaderProvider(OAuthFlow oAuthFlow, string state)
        {
            _oAuthFlow = oAuthFlow;
            _state = state;
        }

        /// <inheritdoc/>
        public bool IsAuthorized => SafeSync.Run(() => _oAuthFlow.IsAuthenticated(_state));

        /// <inheritdoc/>
        public string GetAuthorizationHeaderValue()
        {
            var header = SafeSync.Run(() => _oAuthFlow.GetAuthorizationHeaderAsync(_state));
            return header.ToString();
        }
    }
}
