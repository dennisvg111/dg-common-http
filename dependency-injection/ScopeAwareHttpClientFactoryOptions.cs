using System;

namespace DG.Common.Http.DependencyInjection
{
    /// <summary>
    /// Provides configuration options for creating HTTP clients with a specific handler type.
    /// </summary>
    /// <remarks>This class allows specifying the type of HTTP handler to be used when creating HTTP clients. The specified handler type must derive from <see cref="System.Net.Http.HttpMessageHandler"/>.</remarks>
    public class ScopeAwareHttpClientFactoryOptions
    {
        /// <summary>
        /// Gets or sets the type of the HTTP handler associated with the request.
        /// </summary>
        public Type HttpHandlerType { get; set; }
    }
}