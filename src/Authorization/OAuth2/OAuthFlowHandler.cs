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
    public class OAuthFlowHandler<T> where T : IOAuthLogic
    {
        private readonly object _repositoryLock = new object();

        private readonly T _logic;
        private readonly IOAuthRepository _repository;

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthFlowHandler{T}"/> with the given <see cref="IOAuthLogic"/> and <see cref="IOAuthRepository"/>.
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="repository"></param>
        public OAuthFlowHandler(T logic, IOAuthRepository repository)
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
        public OAuthFlow<T> ForState(string state)
        {
            if (!_repository.TryGetByState(state, out OAuthData data))
            {
                OAuthFlowNotFoundException.ThrowForState(state);
            }
            var flow = OAuthFlow.ContinueFor(_logic, data);
            flow.OnUpdate += Flow_OnUpdate;

            return flow;
        }

        /// <summary>
        /// <para>Returns a new authorization flow with a newly generated state value, for the given <paramref name="scopes"/> and <paramref name="callBackUri"/>.</para>
        /// <para>Any updates during this flow will be automatically saved to the given <see cref="IOAuthRepository"/>.</para>
        /// <para>Note that <see cref="OAuthFlow{T}.State"/> can be used to retrieve this authorization flow using <see cref="ForState(string)"/>, if needed.</para>
        /// </summary>
        /// <param name="scopes"></param>
        /// <param name="callBackUri"></param>
        /// <returns></returns>
        public OAuthFlow<T> StartNewFlow(string[] scopes, Uri callBackUri)
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
                    CallBackUri = callBackUri
                };

                return StartNewFlow(request);
            }
        }

        /// <summary>
        /// <para>Returns a new authorization flow for the given <paramref name="request"/>.</para>
        /// <para>Any updates during this flow will be automatically saved to the given <see cref="IOAuthRepository"/>.</para>
        /// <para>Note that <see cref="OAuthFlow{T}.State"/> can be used to retrieve this authorization flow using <see cref="ForState(string)"/>.</para>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OAuthFlow<T> StartNewFlow(OAuthRequest request)
        {
            var flow = OAuthFlow.StartNewFor(_logic, request);

            _repository.Save(flow.Export());
            flow.OnUpdate += Flow_OnUpdate;

            return flow;
        }

        private void Flow_OnUpdate(object sender, UpdateEventArgs e)
        {
            _repository.Save(OAuthData.From(e.Data));
        }

        /// <summary>
        /// Used to construct new instances of <see cref="OAuthFlowHandler"/>.
        /// </summary>
        public class OAuthFlowHandlerBuilder
        {
            private readonly T _logic;

            internal OAuthFlowHandlerBuilder(T logic)
            {
                _logic = logic;
            }

            /// <summary>
            /// Returns a new instance of <see cref="OAuthFlowHandler"/> with the previously given <see cref="IOAuthLogic"/> implementation and the given <paramref name="repository"/>
            /// </summary>
            /// <param name="repository"></param>
            /// <returns></returns>
            public OAuthFlowHandler<T> AndWith(IOAuthRepository repository)
            {
                return new OAuthFlowHandler<T>(_logic, repository);
            }

            /// <summary>
            /// Returns a new instance of <see cref="OAuthFlowHandler"/> with the previously given <see cref="IOAuthLogic"/> and <see cref="OAuthMemoryRepository.Instance"/> as repository.
            /// </summary>
            /// <returns></returns>
            public OAuthFlowHandler<T> AndInMemoryRepository()
            {
                return new OAuthFlowHandler<T>(_logic, OAuthMemoryRepository.Instance);
            }
        }
    }

    /// <summary>
    /// Provides methods for creating a new instance of <see cref="OAuthFlowHandler{T}"/>
    /// </summary>
    public static class OAuthFlowHandler
    {
        /// <summary>
        /// Starts constructing a new instance of <see cref="OAuthFlowHandler"/> with the given <paramref name="logic"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logic"></param>
        /// <returns></returns>
        public static OAuthFlowHandler<T>.OAuthFlowHandlerBuilder With<T>(T logic) where T : IOAuthLogic
        {
            return new OAuthFlowHandler<T>.OAuthFlowHandlerBuilder(logic);
        }

        /// <summary>
        /// Starts constructing a new instance of <see cref="OAuthFlowHandler"/> with a new instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static OAuthFlowHandler<T>.OAuthFlowHandlerBuilder For<T>() where T : IOAuthLogic, new()
        {
            var logic = new T();
            return With(logic);
        }
    }
}
