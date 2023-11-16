using DG.Common.Http.Extensions;
using System.Net.Http.Headers;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Provides extension methods with <see cref="FluentHeader"/> parameters for <see cref="HttpHeaders"/>.
    /// </summary>
    public static class HttpHeadersExtensions
    {
        /// <summary>
        /// Returns a value that indicates whether the specified <paramref name="header"/> was added to the <see cref="HttpHeaders"/> without validating the provided information.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="header"></param>
        /// <returns>Returns <see langword="true"/> if the specified <paramref name="header"/> could be added to the collection; otherwise <see langword="false"/>.</returns>
        public static bool TryAddWithoutValidation(this HttpHeaders headers, FluentHeader header)
        {
            return headers.TryAddWithoutValidation(header.Name, header.Value);
        }

        /// <summary>
        /// Adds the specified <paramref name="header"/> into the <see cref="HttpHeaders"/> collection.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="header"></param>
        public static void Add(this HttpHeaders headers, FluentHeader header)
        {
            headers.Add(header.Name, header.Value);
        }

        /// <summary>
        /// Adds the specified <paramref name="header"/> into the <see cref="HttpHeaders"/> collection, replacing any existing header with the same name.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="header"></param>
        public static void AddOrReplace(this HttpHeaders headers, FluentHeader header)
        {
            headers.AddOrReplace(header.Name, header.Value);
        }
    }
}
