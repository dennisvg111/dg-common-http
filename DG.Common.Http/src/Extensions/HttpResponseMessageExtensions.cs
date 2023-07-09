using System.Linq;
using System.Net;
using System.Net.Http;

namespace DG.Common.Http.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        private static readonly HttpStatusCode[] _redirectCodes = new[] { (HttpStatusCode)301, (HttpStatusCode)302, (HttpStatusCode)303, (HttpStatusCode)307, (HttpStatusCode)308 };

        /// <summary>
        /// Indicates if this response is a redirect.
        /// </summary>
        /// <param name="response"></param>
        public static bool IsRedirect(this HttpResponseMessage response)
        {
            if (!_redirectCodes.Contains(response.StatusCode))
            {
                return false;
            }
            if (response.Headers.Location == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Indicates whether this redirect should change the method to GET.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="originalMethod"></param>
        /// <returns></returns>
        public static bool IsGetRedirect(this HttpResponseMessage response, HttpMethod originalMethod)
        {
            switch (response.StatusCode)
            {
                // Current standard for HttpClient is changing 301 and 302 to GET only if it is a POST (even against specification).
                case (HttpStatusCode)301:
                case (HttpStatusCode)302:
                    return originalMethod == HttpMethod.Post;
                case (HttpStatusCode)303:
                    return true;
                default:
                    return false;
            }
        }
    }
}
