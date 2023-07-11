using System.Net.Http.Headers;

namespace DG.Common.Http.Extensions
{
    public static class HttpRequestHeadersExtensions
    {
        public static void AddOrReplace(this HttpRequestHeaders headers, string name, string value)
        {
            if (headers.Contains(name))
            {
                headers.Remove(name);
            }
            headers.Add(name, value);
        }
    }
}
