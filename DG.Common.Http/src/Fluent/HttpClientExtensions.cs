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
        /// <param name="messageBuilder">The builder used to create a <see cref="HttpRequestMessage"/>.</param>
        /// <returns>Returns a <see cref="Task{TResult}"/> object representing the asynchronous operation.</returns>
        public static async Task<HttpResponseMessage> SendMessageAsync(this HttpClient client, FluentRequest messageBuilder)
        {
            return await FollowRedirects(client, messageBuilder);
        }

        private static async Task<HttpResponseMessage> FollowRedirects(HttpClient client, FluentRequest messageBuilder)
        {
            var request = messageBuilder.MessageForClient(client);

            var response = await client.SendAsync(request);
            if (messageBuilder.CookieJar != null)
            {
                messageBuilder.CookieJar.CollectFrom(response);
            }

            if (response.IsRedirect() && messageBuilder.MaxRedirects > 0)
            {
                var redirect = messageBuilder.RedirectForResponse(response);
                return await FollowRedirects(client, redirect);
            }

            return response;
        }
    }
}
