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
        /// Send the HTTP request created by the given <see cref="FluentRequest"/> as an asynchronous operation.
        /// </summary>
        /// <param name="client">The client used to send a <see cref="HttpRequestMessage"/>.</param>
        /// <param name="request">The <see cref="FluentRequest"/> used to create a <see cref="HttpRequestMessage"/>.</param>
        /// <returns>Returns a <see cref="Task{TResult}"/> object representing the asynchronous operation.</returns>
        public static async Task<HttpResponseMessage> SendMessageAsync(this HttpClient client, FluentRequest request)
        {
            var message = request.MessageForClient(client);

            var response = await client.SendAsync(message);
            request.CollectCookiesIfNeeded(response);

            if (response.IsRedirect() && request.MaxRedirects > 0)
            {
                var redirect = request.RedirectForResponse(response);
                return await SendMessageAsync(client, redirect);
            }

            return response;
        }
    }
}
