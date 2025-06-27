using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace DG.Common.Http.DependencyInjection
{
    // Based on https://github.com/dotnet/docs/blob/main/docs/core/extensions/snippets/http/scopeworkaround/ScopeAwareHttpClientFactory.cs

    /// <summary>
    /// Provides an implementation of <see cref="IHttpClientFactory"/> that creates <see cref="HttpClient"/> instances with support for scope-aware delegating handlers.
    /// </summary>
    public class ScopeAwareHttpClientFactory : IHttpClientFactory
    {
        private readonly IServiceProvider _scopeServiceProvider;
        private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory; // using IHttpMessageHandlerFactory to get access to HttpClientFactory's cached handlers
        private readonly IOptionsMonitor<HttpClientFactoryOptions> _clientFactoryOptionsMonitor;
        private readonly IOptionsMonitor<ScopeAwareHttpClientFactoryOptions> _scopeAwareOptionsMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeAwareHttpClientFactory"/> class.
        /// </summary>
        /// <param name="scopeServiceProvider">The <see cref="IServiceProvider"/> used to resolve scoped services for configuring <see cref="HttpClient"/> instances.</param>
        /// <param name="httpMessageHandlerFactory">The <see cref="IHttpMessageHandlerFactory"/> used to create <see cref="HttpMessageHandler"/> instances for the <see cref="HttpClient"/>.</param>
        /// <param name="clientFactoryOptionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> for monitoring changes to <see cref="HttpClientFactoryOptions"/>.</param>
        /// <param name="scopeAwareOptionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> for monitoring changes to <see cref="ScopeAwareHttpClientFactoryOptions"/>.</param>
        public ScopeAwareHttpClientFactory(
            IServiceProvider scopeServiceProvider,
            IHttpMessageHandlerFactory httpMessageHandlerFactory,
            IOptionsMonitor<HttpClientFactoryOptions> clientFactoryOptionsMonitor,
            IOptionsMonitor<ScopeAwareHttpClientFactoryOptions> scopeAwareOptionsMonitor)
        {
            _scopeServiceProvider = scopeServiceProvider;
            _httpMessageHandlerFactory = httpMessageHandlerFactory;
            _clientFactoryOptionsMonitor = clientFactoryOptionsMonitor;
            _scopeAwareOptionsMonitor = scopeAwareOptionsMonitor;
        }

        /// <inheritdoc />
        public HttpClient CreateClient(string name)
        {
            // Get custom options to get scope aware handler information
            ScopeAwareHttpClientFactoryOptions scopeAwareOptions = _scopeAwareOptionsMonitor.Get(name);
            Type scopeAwareHandlerType = scopeAwareOptions.HttpHandlerType;

            // Get or create HttpMessageHandler from HttpClientFactory
            HttpMessageHandler handler = _httpMessageHandlerFactory.CreateHandler(name);

            var scopeAwareHandler = GetHandlerForType(scopeAwareHandlerType);
            if (scopeAwareHandler != null)
            {
                scopeAwareHandler.InnerHandler = handler;
            }

            HttpClient client = new HttpClient(scopeAwareHandler ?? handler);

            RunDefaultFactoryConfiguration(client, name);

            return client;
        }

        private DelegatingHandler GetHandlerForType(Type handlerType)
        {
            if (handlerType == null)
            {
                return null;
            }

            DelegatingHandler scopeAwareHandler = null;
            if (!typeof(DelegatingHandler).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException($"Scope aware HttpHandler {handlerType.Name} should be assignable to {nameof(DelegatingHandler)}");
            }

            // Create top-most delegating handler with scoped dependencies
            scopeAwareHandler = (DelegatingHandler)_scopeServiceProvider.GetRequiredService(handlerType); // should be transient
            if (scopeAwareHandler.InnerHandler != null)
            {
                throw new ArgumentException($"Inner handler of a delegating handler {handlerType.Name} should be null. Scope aware HttpHandler should be registered as Transient.");
            }
            return scopeAwareHandler;
        }

        /// <summary>
        /// Runs the default factory configuration actions based on the registered <see cref="HttpClientFactoryOptions"/>, using <see cref="IOptionsMonitor{TOptions}"/>.
        /// </summary>
        /// <param name="client">The <see cref="HttpClient"/> to configure.</param>
        /// <param name="name">The name of the <see cref="HttpClientFactoryOptions"/> to use.</param>
        protected void RunDefaultFactoryConfiguration(HttpClient client, string name)
        {
            var configurationActions = _clientFactoryOptionsMonitor.Get(name).HttpClientActions;
            for (int i = 0; i < configurationActions.Count; i++)
            {
                configurationActions[i](client);
            }
        }
    }
}