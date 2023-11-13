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
        /// <para>Returns an existing authorization flow from the repository with the given state.</para>
        /// <para>Any updates during this flow will be automatically saved to the given <see cref="IOAuthRepository"/>.</para>
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <exception cref="OAuthFlowNotFoundException"></exception>
        public OAuthFlow ForState(string state)
        {
            if (!_repository.TryGetByState(state, out OAuthData data))
            {
                OAuthFlowNotFoundException.ThrowForState(state);
            }
            var flow = new OAuthFlow(_logic, data);

            SetupOnUpdate(flow);
            return flow;
        }

        /// <summary>
        /// <para>Returns a new authorization flow with a newly generated state value, for the given <paramref name="scopes"/> and <paramref name="redirectUri"/>.</para>
        /// <para>Any updates during this flow will be automatically saved to the given <see cref="IOAuthRepository"/>.</para>
        /// <para>Note that <see cref="OAuthFlow.State"/> can be used to retrieve this authorization flow using <see cref="ForState(string)"/>, if needed.</para>
        /// </summary>
        /// <param name="scopes"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public OAuthFlow StartNewFlow(string[] scopes, Uri redirectUri)
        {
            lock (_repositoryLock)
            {
                string state;
                do
                {
                    state = OAuthState.NewState();
                } while (_repository.TryGetByState(state, out OAuthData _));

                var request = new OAuthRequest()
                {
                    State = state,
                    Scopes = scopes,
                    RedirectUri = redirectUri
                };

                return StartNewFlow(request);
            }
        }

        /// <summary>
        /// <para>Returns a new authorization flow for the given <paramref name="request"/>.</para>
        /// <para>Any updates during this flow will be automatically saved to the given <see cref="IOAuthRepository"/>.</para>
        /// <para>Note that <see cref="OAuthFlow.State"/> can be used to retrieve this authorization flow using <see cref="ForState(string)"/>.</para>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OAuthFlow StartNewFlow(OAuthRequest request)
        {
            var flow = _logic.StartNewFlow(request);

            _repository.Save(flow.Export());

            SetupOnUpdate(flow);
            return flow;
        }

        private void SetupOnUpdate(OAuthFlow flow)
        {
            //Prevent Flow_OnUpdate being called twice
            flow.OnUpdate -= Flow_OnUpdate;
            flow.OnUpdate += Flow_OnUpdate;
        }

        private void Flow_OnUpdate(object sender, UpdateEventArgs e)
        {
            _repository.Save(OAuthData.From(e.Data));
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
