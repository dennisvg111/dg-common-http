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
        /// Adds the specified <paramref name="header"/> into the <see cref="HttpHeaders"/> collection.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="header"></param>
        public static void Add(this HttpHeaders headers, FluentHeader header)
        {
            headers.AddWithOptionalValidation(header.Name, header.Value, header.SkipValidation);
        }

        /// <summary>
        /// Adds the specified <paramref name="header"/> into the <see cref="HttpHeaders"/> collection, replacing any existing header with the same name.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="header"></param>
        public static void AddOrReplace(this HttpHeaders headers, FluentHeader header)
        {
            headers.AddOrReplace(header.Name, header.Value, header.SkipValidation);
        }
    }
}
