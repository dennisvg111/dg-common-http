using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Http.Authorization.OAuth2.Exceptions;
using DG.Common.Http.Authorization.OAuth2.Interfaces;
using System;

namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// <para>Provides functionality to authorize using an OAuth2 authorization flow.</para>
    /// <para>Note that this class can handle multiple users authorizing, based on each user having a unique state.</para>
    /// </summary>
    public class OAuthFlowHandler
    {
        private readonly object _repositoryLock = new object();

        private readonly IOAuthLogic _logic;
        private readonly IOAuthRepository _repository;

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthFlowHandler"/> with the given <see cref="IOAuthLogic"/> and <see cref="IOAuthRepository"/>.
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="repository"></param>
        public OAuthFlowHandler(IOAuthLogic logic, IOAuthRepository repository)
        {
            _logic = logic;
            _repository = repository;
        }

        /// <summary>
        /// Returns an authorization flow from the repository for the request with the given state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <exception cref="OAuthFlowNotFoundException"></exception>
        public OAuthFlow ForState(string state)
        {
            if (!_repository.TryGetRequestByState(state, out OAuthData request))
            {
                OAuthFlowNotFoundException.ThrowForState(state);
            }
            return new OAuthFlow(_logic, request);
        }

        /// <summary>
        /// <para>Returns the authorization url created for the given <paramref name="scopes"/> and <paramref name="redirectUri"/>.</para>
        /// <para>Note that <see cref="OAuthFlow.State"/> can be used to call <see cref="AuthenticationCallback(string, string)"/> and <see cref="GetAuthorizationHeaderAsync(string)"/> later.</para>
        /// </summary>
        /// <param name="scopes"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public OAuthFlow StartNewFlow(string[] scopes, Uri redirectUri)
        {
            OAuthData savedRequest;
            lock (_repositoryLock)
            {
                OAuthRequest request;
                do
                {
                    request = OAuthRequest.NewFor(scopes, redirectUri);
                } while (_repository.TryGetRequestByState(request.State, out OAuthData _));

                savedRequest = OAuthData.From(request);
                _repository.SaveRequest(savedRequest);
            }

            var flow = new OAuthFlow(_logic, savedRequest);
            flow.OnRefresh += Flow_OnRefresh;

            return flow;
        }

        private void Flow_OnRefresh(object sender, RefreshEventArgs e)
        {
            _repository.SaveRequest(OAuthData.From(e.Request));
        }

        /// <summary>
        /// Starts constructing a new instance of <see cref="OAuthFlowHandler"/> with the given <paramref name="logic"/>.
        /// </summary>
        /// <param name="logic"></param>
        /// <returns></returns>
        public static OAuthFlowHandlerBuilder With(IOAuthLogic logic)
        {
            return new OAuthFlowHandlerBuilder(logic);
        }

        /// <summary>
        /// Used to construct new instances of <see cref="OAuthFlowHandler"/>.
        /// </summary>
        public class OAuthFlowHandlerBuilder
        {
            private readonly IOAuthLogic _logic;

            internal OAuthFlowHandlerBuilder(IOAuthLogic logic)
            {
                _logic = logic;
            }

            /// <summary>
            /// Returns a new instance of <see cref="OAuthFlowHandler"/> with the previously given <see cref="IOAuthLogic"/> implementation and the given <paramref name="repository"/>
            /// </summary>
            /// <param name="repository"></param>
            /// <returns></returns>
            public OAuthFlowHandler AndWith(IOAuthRepository repository)
            {
                return new OAuthFlowHandler(_logic, repository);
            }

            /// <summary>
            /// Returns a new instance of <see cref="OAuthFlowHandler"/> with the previously given <see cref="IOAuthLogic"/> and <see cref="OAuthMemoryRepository.Instance"/> as repository.
            /// </summary>
            /// <returns></returns>
            public OAuthFlowHandler AndInMemoryRepository()
            {
                return new OAuthFlowHandler(_logic, OAuthMemoryRepository.Instance);
            }
        }
    }
}
