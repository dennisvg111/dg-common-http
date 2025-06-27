using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System.Net.Http;

namespace DG.Common.Http.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for configuring an <see cref="IHttpClientBuilder"/> to use scope-aware HTTP handlers.
    /// </summary>
    /// <remarks>These extensions allow the registration of a custom <see cref="DelegatingHandler"/> that is scope-aware, ensuring that the handler is properly resolved and managed within the dependency injection scope.</remarks>
    public static class ScopeAwareHttpClientBuilderExtensions
    {
        /// <summary>
        /// Adds a scope-aware HTTP handler of the specified type to the HTTP client pipeline.
        /// </summary>
        /// <remarks>
        /// This method registers the specified <typeparamref name="THandler"/> transient service and ensures that the <see cref="ScopeAwareHttpClientFactory"/> is used as the <see cref="IHttpClientFactory"/> implementation for the HTTP client.
        /// The handler type is associated with the named HTTP client being configured.
        /// </remarks>
        /// <typeparam name="THandler">The type of the delegating handler to add. Must derive from <see cref="DelegatingHandler"/>.</typeparam>
        /// <param name="builder">The <see cref="IHttpClientBuilder"/> to configure.</param>
        /// <returns>The <see cref="IHttpClientBuilder"/> instance, allowing further configuration.</returns>
        public static IHttpClientBuilder AddScopeAwareHttpHandler<THandler>(this IHttpClientBuilder builder)
            where THandler : DelegatingHandler
        {
            builder.Services.TryAddTransient<THandler>();
            if (!builder.Services.Any(sd => sd.ImplementationType == typeof(ScopeAwareHttpClientFactory)))
            {
                // Override default IHttpClientFactory registration
                builder.Services.AddTransient<IHttpClientFactory, ScopeAwareHttpClientFactory>();
            }

            builder.Services.Configure<ScopeAwareHttpClientFactoryOptions>(builder.Name, options => options.HttpHandlerType = typeof(THandler));

            return builder;
        }
    }
}