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
        /// <param name="skipValidation"></param>
        public static void AddOrReplace(this HttpHeaders headers, string name, string value, bool skipValidation = false)
        {
            if (headers.Contains(name))
            {
                headers.Remove(name);
            }
            AddWithOptionalValidation(headers, name, value, skipValidation);
        }

        /// <summary>
        /// Adds the specified header and its value into the <see cref="HttpHeaders"/>, optionally skipping validation.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="skipValidation"></param>
        public static void AddWithOptionalValidation(this HttpHeaders headers, string name, string value, bool skipValidation)
        {
            if (skipValidation)
            {
                headers.TryAddWithoutValidation(name, value);
                return;
            }
            headers.Add(name, value);
        }
    }
}
