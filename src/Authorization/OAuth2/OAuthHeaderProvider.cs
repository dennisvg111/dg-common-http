using DG.Common.Threading;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// Provides <c>Authorization</c> header values for HTTP requests.
    /// </summary>
    public class OAuthHeaderProvider : IAuthorizationHeaderProvider
    {
        private readonly OAuthFlowHandler _oAuthFlow;
        private readonly string _state;

        /// <summary>
        /// Initializes a new instanceo of <see cref="OAuthHeaderProvider"/> with the given <see cref="OAuthFlowHandler"/> and <paramref name="state"/>.
        /// </summary>
        /// <param name="oAuthFlow"></param>
        /// <param name="state"></param>
        public OAuthHeaderProvider(OAuthFlowHandler oAuthFlow, string state)
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
