using DG.Common.Http.Fluent;
using System.Net.Http.Headers;

namespace DG.Common.Http.Extensions
{
    public static class HttpHeadersExtensions
    {
        public static void AddOrReplace(this HttpHeaders headers, string name, string value)
        {
            if (headers.Contains(name))
            {
                headers.Remove(name);
            }
            headers.Add(name, value);
        }

        public static void AddOrReplace(this HttpHeaders headers, FluentHeader header)
        {
            headers.AddOrReplace(header.Name, header.Value);
        }
    }
}
