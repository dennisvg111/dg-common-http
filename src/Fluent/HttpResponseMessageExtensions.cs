using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Provides extension methods for <see cref="HttpResponseMessage"/>
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Deserializes the JSON content of this response to the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<T> DeserializeResponseAsync<T>(this HttpResponseMessage response)
        {
            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
