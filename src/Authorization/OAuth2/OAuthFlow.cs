using DG.Common.Http.Authorization.OAuth2.Exceptions;
using System;
using System.Threading.Tasks;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// Provides functionality to authorize users using an OAuth2 authorization flow.
    /// </summary>
    public class OAuthFlow
    {
        private readonly IOAuthLogic _logic;
        private readonly IOAuthRepository _repository;

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthFlow"/> with the given <see cref="IOAuthLogic"/> and <see cref="IOAuthRepository"/>.
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="repository"></param>
        public OAuthFlow(IOAuthLogic logic, IOAuthRepository repository)
        {
            _logic = logic;
            _repository = repository;
        }

        private string GenerateState()
        {
            return Uulsid.NewUulsid().ToString();
        }

        /// <summary>
        /// <para>Returns the authorization url created for the given <paramref name="scopes"/> and <paramref name="redirectUri"/>.</para>
        /// <para>Note that <paramref name="state"/> will be used to call <see cref="AuthenticationCallback(string, string)"/> and <see cref="GetAuthorizationHeaderAsync(string)"/> later.</para>
        /// </summary>
        /// <param name="scopes"></param>
        /// <param name="redirectUri"></param>
        /// <param name="state">The state value of the new authorization request.</param>
        /// <returns></returns>
        public Uri GetAuthorizationUri(string[] scopes, Uri redirectUri, out string state)
        {
            do
            {
                state = GenerateState();
            } while (_repository.TryGetRequestByState(state, out SavedOAuthRequest _));

            var request = new SavedOAuthRequest()
            {
                State = state,
                RedirectUrl = redirectUri,
                Scopes = scopes,
                Started = DateTimeOffset.UtcNow
            };

            _repository.SaveRequest(request);

            return _logic.BuildAuthenticationUrlFor(request);
        }

        /// <summary>
        /// Processes the given <paramref name="code"/>, and saves the resulting <see cref="OAuthToken"/> to the current repository.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task AuthenticationCallback(string state, string code)
        {
            if (!_repository.TryGetRequestByState(state, out SavedOAuthRequest request))
            {
                OAuthRequestNotFoundException.ThrowForState(state);
            }

            var token = await _logic.GetAccessTokenAsync(request, code).ConfigureAwait(false);
            request.UpdateWith(token);
            _repository.SaveRequest(request);
        }

        /// <summary>
        /// Returns a value indicating if authorization is granted for the given <paramref name="state"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<bool> IsAuthenticated(string state)
        {
            if (!_repository.TryGetRequestByState(state, out SavedOAuthRequest request))
            {
                return false;
            }

            if (!request.IsCompleted)
            {
                return false;
            }

            if (request.ValidUntill >= DateTimeOffset.UtcNow)
            {
                return true;
            }

            return await RefreshRequestAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a new <see cref="AuthorizationHeaderValue"/> for the given <paramref name="state"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <exception cref="OAuthRequestNotFoundException"></exception>
        /// <exception cref="OAuthRequestNotCompletedException"></exception>
        /// <exception cref="OAuthAuthorizationExpiredException"></exception>
        public async Task<AuthorizationHeaderValue> GetAuthorizationHeaderAsync(string state)
        {
            if (!_repository.TryGetRequestByState(state, out SavedOAuthRequest request))
            {
                OAuthRequestNotFoundException.ThrowForState(state);
            }

            if (!request.IsCompleted)
            {
                OAuthRequestNotCompletedException.ThrowForState(state);
            }

            if (request.ValidUntill < DateTimeOffset.UtcNow)
            {
                var canRefresh = await RefreshRequestAsync(request).ConfigureAwait(false);
                if (!canRefresh)
                {
                    OAuthAuthorizationExpiredException.ThrowForState(state);
                }
            }

            return _logic.GetHeaderForToken(request.AccessToken);
        }

        private async Task<bool> RefreshRequestAsync(SavedOAuthRequest request)
        {
            var canRefresh = await _logic.RefreshTokenAsync(request.RefreshToken, out OAuthToken token).ConfigureAwait(false);
            if (!canRefresh)
            {
                return false;
            }
            _repository.SaveRequest(request);
            request.UpdateWith(token);
            return true;
        }

        /// <summary>
        /// Starts constructing a new instance of <see cref="OAuthFlow"/> with the given <paramref name="logic"/>.
        /// </summary>
        /// <param name="logic"></param>
        /// <returns></returns>
        public static OAuthFlowBuilder With(IOAuthLogic logic)
        {
            return new OAuthFlowBuilder(logic);
        }

        /// <summary>
        /// Used to construct new instances of <see cref="OAuthFlow"/>.
        /// </summary>
        public class OAuthFlowBuilder
        {
            private readonly IOAuthLogic _logic;

            internal OAuthFlowBuilder(IOAuthLogic logic)
            {
                _logic = logic;
            }

            /// <summary>
            /// Returns a new instance of <see cref="OAuthFlow"/> with the previously given <see cref="IOAuthLogic"/> implementation and the given <paramref name="repository"/>
            /// </summary>
            /// <param name="repository"></param>
            /// <returns></returns>
            public OAuthFlow AndWith(IOAuthRepository repository)
            {
                return new OAuthFlow(_logic, repository);
            }

            /// <summary>
            /// Returns a new instance of <see cref="OAuthFlow"/> with the previously given <see cref="IOAuthLogic"/> and <see cref="OAuthMemoryRepository.Instance"/> as repository.
            /// </summary>
            /// <returns></returns>
            public OAuthFlow AndInMemoryRepository()
            {
                return new OAuthFlow(_logic, OAuthMemoryRepository.Instance);
            }
        }
    }
}
