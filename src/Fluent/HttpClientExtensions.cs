using DG.Common.Http.Extensions;
using System.Net.Http;
using System.Threading.Tasks;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// This class provides extension methods for <see cref="HttpClient"/>.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Send an HTTP request based on the given <see cref="FluentRequest"/> as an asynchronous operation.
        /// </summary>
        /// <param name="client">The client used to send a <see cref="HttpRequestMessage"/>.</param>
        /// <param name="request">The <see cref="FluentRequest"/> used to create a HTTP request message.</param>
        /// <returns>Returns a <see cref="Task{TResult}"/> object representing the asynchronous operation.</returns>
        public static async Task<HttpResponseMessage> SendAsync(this HttpClient client, FluentRequest request)
        {
            var message = request.MessageForBaseUri(client.BaseAddress);

            var response = await client.SendAsync(message, request.CompletionOption, request.CancellationToken);
            request.CollectCookiesIfNeeded(response);

            if (response.IsRedirect() && request.MaxRedirects > 0)
            {
                var redirect = request.RedirectForResponse(response);
                return await SendAsync(client, redirect);
            }

            return response;
        }

        /// <summary>
        /// Send an HTTP request based on the given <see cref="FluentRequest"/> as an asynchronous operation, and deserializes the resulting JSON response content to the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<T> SendAndDeserializeAsync<T>(this HttpClient client, FluentRequest request)
        {
            using (var response = await client.SendAsync(request))
            {
                return await response.DeserializeResponseAsync<T>();
            }
        }
    }
}
