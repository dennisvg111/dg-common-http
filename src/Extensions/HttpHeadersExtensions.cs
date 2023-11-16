using System.Net.Http.Headers;

namespace DG.Common.Http.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="HttpHeaders"/>.
    /// </summary>
    public static class HttpHeadersExtensions
    {
        /// <summary>
        /// Adds the specified header and its value into the <see cref="HttpHeaders"/> collection, replacing any existing header with the same name.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void AddOrReplace(this HttpHeaders headers, string name, string value)
        {
            if (headers.Contains(name))
            {
                headers.Remove(name);
            }
            headers.Add(name, value);
        }
    }
}
